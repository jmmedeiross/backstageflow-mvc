using BackstageFlow.Web.Data;
using BackstageFlow.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackstageFlow.Web.Controllers;

public sealed class EventsController(AppDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index()
    {
        List<Event> events = await dbContext.Events.AsNoTracking()
            .Include(item => item.Artists)
            .OrderBy(item => item.StartsAt)
            .ToListAsync();
        return View(events);
    }

    public async Task<IActionResult> Details(int id)
    {
        Event? item = await dbContext.Events.AsNoTracking()
            .Include(candidate => candidate.Artists.OrderBy(artist => artist.PerformanceTime))
            .SingleOrDefaultAsync(candidate => candidate.Id == id);
        return item is null ? NotFound() : View(item);
    }

    public IActionResult Create()
    {
        DateTime start = DateTime.Today.AddDays(7).AddHours(18);
        return View(new Event { StartsAt = start, EndsAt = start.AddHours(6) });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Event item)
    {
        ValidateDates(item);
        if (!ModelState.IsValid)
        {
            return View(item);
        }

        dbContext.Events.Add(item);
        await dbContext.SaveChangesAsync();
        TempData["Success"] = "Evento criado com sucesso.";
        return RedirectToAction(nameof(Details), new { id = item.Id });
    }

    public async Task<IActionResult> Edit(int id)
    {
        Event? item = await dbContext.Events.FindAsync(id);
        return item is null ? NotFound() : View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Event item)
    {
        if (id != item.Id)
        {
            return BadRequest();
        }

        ValidateDates(item);
        if (!ModelState.IsValid)
        {
            return View(item);
        }

        dbContext.Update(item);
        await dbContext.SaveChangesAsync();
        TempData["Success"] = "Evento atualizado.";
        return RedirectToAction(nameof(Details), new { id = item.Id });
    }

    public async Task<IActionResult> Delete(int id)
    {
        Event? item = await dbContext.Events.AsNoTracking()
            .Include(candidate => candidate.Artists)
            .SingleOrDefaultAsync(candidate => candidate.Id == id);
        return item is null ? NotFound() : View(item);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        Event? item = await dbContext.Events.FindAsync(id);
        if (item is null)
        {
            return NotFound();
        }

        dbContext.Events.Remove(item);
        await dbContext.SaveChangesAsync();
        TempData["Success"] = "Evento removido.";
        return RedirectToAction(nameof(Index));
    }

    private void ValidateDates(Event item)
    {
        if (item.EndsAt <= item.StartsAt)
        {
            ModelState.AddModelError(nameof(item.EndsAt), "O término precisa ser posterior ao início.");
        }
    }
}
