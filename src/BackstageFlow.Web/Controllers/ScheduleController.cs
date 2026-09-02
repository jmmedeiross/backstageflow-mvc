using BackstageFlow.Web.Data;
using BackstageFlow.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackstageFlow.Web.Controllers;

public sealed class ScheduleController(AppDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index(int? eventId)
    {
        var query = dbContext.Artists.AsNoTracking().Include(item => item.Event).AsQueryable();
        if (eventId.HasValue)
        {
            query = query.Where(item => item.EventId == eventId.Value);
        }

        var model = new ScheduleViewModel
        {
            Artists = await query.OrderBy(item => item.PerformanceTime).ToListAsync(),
            EventId = eventId,
            Events = await dbContext.Events.AsNoTracking().OrderBy(item => item.StartsAt).ToListAsync()
        };
        return View(model);
    }
}
