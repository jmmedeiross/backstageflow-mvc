using BackstageFlow.Web.Data;
using BackstageFlow.Web.Models;
using BackstageFlow.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BackstageFlow.Web.Controllers;

public sealed class ArtistsController(AppDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(int? eventId, ArtistStatus? status, string? search)
    {
        IQueryable<ArtistBooking> query = dbContext.Artists.AsNoTracking().Include(item => item.Event);
        if (eventId.HasValue)
        {
            query = query.Where(item => item.EventId == eventId.Value);
        }
        if (status.HasValue)
        {
            query = query.Where(item => item.Status == status.Value);
        }
        if (!string.IsNullOrWhiteSpace(search))
        {
            string term = search.Trim();
            query = query.Where(item => EF.Functions.Like(item.StageName, $"%{term}%"));
        }

        var model = new ArtistListViewModel
        {
            Artists = await query.OrderBy(item => item.ArrivalTime).ToListAsync(),
            Events = await dbContext.Events.AsNoTracking().OrderBy(item => item.StartsAt).ToListAsync(),
            EventId = eventId,
            Status = status,
            Search = search
        };
        return View(model);
    }

    public async Task<IActionResult> Details(int id)
    {
        ArtistBooking? item = await dbContext.Artists.AsNoTracking()
            .Include(candidate => candidate.Event)
            .SingleOrDefaultAsync(candidate => candidate.Id == id);
        return item is null ? NotFound() : View(item);
    }

    public async Task<IActionResult> Create(int? eventId)
    {
        await LoadEventsAsync(eventId);
        DateTime arrival = DateTime.Today.AddDays(7).AddHours(17);
        return View(new ArtistBooking
        {
            EventId = eventId ?? 0,
            ArrivalTime = arrival,
            PerformanceTime = arrival.AddHours(3)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ArtistBooking item, ArtistStatus selectedStatus)
    {
        await ValidateArtistAsync(item);
        if (!ModelState.IsValid)
        {
            await LoadEventsAsync(item.EventId);
            ViewData["selectedStatus"] = selectedStatus;
            return View(item);
        }

        item.SetStatus(selectedStatus);
        dbContext.Artists.Add(item);
        await dbContext.SaveChangesAsync();
        TempData["Success"] = "Artista adicionado ao evento.";
        return RedirectToAction(nameof(Details), new { id = item.Id });
    }

    public async Task<IActionResult> Edit(int id)
    {
        ArtistBooking? item = await dbContext.Artists.FindAsync(id);
        if (item is null)
        {
            return NotFound();
        }
        await LoadEventsAsync(item.EventId);
        ViewData["selectedStatus"] = item.Status;
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ArtistBooking input, ArtistStatus selectedStatus)
    {
        if (id != input.Id)
        {
            return BadRequest();
        }

        await ValidateArtistAsync(input);
        if (!ModelState.IsValid)
        {
            await LoadEventsAsync(input.EventId);
            ViewData["selectedStatus"] = selectedStatus;
            return View(input);
        }

        ArtistBooking? item = await dbContext.Artists.FindAsync(id);
        if (item is null)
        {
            return NotFound();
        }

        item.StageName = input.StageName;
        item.ContactName = input.ContactName;
        item.ContactEmail = input.ContactEmail;
        item.Phone = input.Phone;
        item.ArrivalTime = input.ArrivalTime;
        item.PerformanceTime = input.PerformanceTime;
        item.SetDurationMinutes = input.SetDurationMinutes;
        item.DressingRoom = input.DressingRoom;
        item.HospitalityNotes = input.HospitalityNotes;
        item.TransportDetails = input.TransportDetails;
        item.EventId = input.EventId;
        item.SetStatus(selectedStatus);

        await dbContext.SaveChangesAsync();
        TempData["Success"] = "Dados do artista atualizados.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckIn(int id, string? returnUrl)
    {
        ArtistBooking? item = await dbContext.Artists.FindAsync(id);
        if (item is null)
        {
            return NotFound();
        }

        try
        {
            item.CheckIn();
            await dbContext.SaveChangesAsync();
            TempData["Success"] = $"Check-in de {item.StageName} realizado.";
        }
        catch (InvalidOperationException exception)
        {
            TempData["Error"] = exception.Message;
        }

        return LocalRedirect(returnUrl ?? Url.Action(nameof(Index))!);
    }

    public async Task<IActionResult> Delete(int id)
    {
        ArtistBooking? item = await dbContext.Artists.AsNoTracking()
            .Include(candidate => candidate.Event)
            .SingleOrDefaultAsync(candidate => candidate.Id == id);
        return item is null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        ArtistBooking? item = await dbContext.Artists.FindAsync(id);
        if (item is null)
        {
            return NotFound();
        }
        dbContext.Artists.Remove(item);
        await dbContext.SaveChangesAsync();
        TempData["Success"] = "Artista removido do evento.";
        return RedirectToAction(nameof(Index));
    }

    private async Task LoadEventsAsync(int? selectedId)
    {
        List<Event> events = await dbContext.Events.AsNoTracking().OrderBy(item => item.StartsAt).ToListAsync();
        ViewBag.Events = new SelectList(events, nameof(Event.Id), nameof(Event.Name), selectedId);
    }

    private async Task ValidateArtistAsync(ArtistBooking item)
    {
        if (!await dbContext.Events.AnyAsync(candidate => candidate.Id == item.EventId))
        {
            ModelState.AddModelError(nameof(item.EventId), "Selecione um evento válido.");
        }
        if (item.PerformanceTime < item.ArrivalTime)
        {
            ModelState.AddModelError(nameof(item.PerformanceTime), "A apresentação não pode acontecer antes da chegada.");
        }
    }
}
