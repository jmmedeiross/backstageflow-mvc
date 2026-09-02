using System.Diagnostics;
using BackstageFlow.Web.Data;
using BackstageFlow.Web.Models;
using BackstageFlow.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackstageFlow.Web.Controllers;

public sealed class HomeController(AppDbContext dbContext) : Controller
{
    public async Task<IActionResult> Index()
    {
        DateTime now = DateTime.Now;
        var model = new DashboardViewModel
        {
            TotalEvents = await dbContext.Events.CountAsync(),
            ConfirmedArtists = await dbContext.Artists.CountAsync(item => item.Status == ArtistStatus.Confirmed),
            AwaitingArrival = await dbContext.Artists.CountAsync(item =>
                item.Status == ArtistStatus.Invited || item.Status == ArtistStatus.Confirmed),
            CheckedInArtists = await dbContext.Artists.CountAsync(item => item.CheckedInAtUtc != null),
            NextEvent = await dbContext.Events.AsNoTracking()
                .Where(item => item.EndsAt >= now)
                .OrderBy(item => item.StartsAt)
                .FirstOrDefaultAsync(),
            UpcomingSchedule = await dbContext.Artists.AsNoTracking()
                .Include(item => item.Event)
                .Where(item => item.PerformanceTime >= now && item.Status != ArtistStatus.Cancelled)
                .OrderBy(item => item.PerformanceTime)
                .Take(6)
                .ToListAsync()
        };

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
