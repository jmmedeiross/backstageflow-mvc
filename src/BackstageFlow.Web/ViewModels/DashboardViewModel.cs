using BackstageFlow.Web.Models;

namespace BackstageFlow.Web.ViewModels;

public sealed class DashboardViewModel
{
    public int TotalEvents { get; init; }
    public int ConfirmedArtists { get; init; }
    public int AwaitingArrival { get; init; }
    public int CheckedInArtists { get; init; }
    public Event? NextEvent { get; init; }
    public IReadOnlyList<ArtistBooking> UpcomingSchedule { get; init; } = [];
}
