using BackstageFlow.Web.Models;

namespace BackstageFlow.Web.ViewModels;

public sealed class ScheduleViewModel
{
    public IReadOnlyList<ArtistBooking> Artists { get; init; } = [];
    public int? EventId { get; init; }
    public IReadOnlyList<Event> Events { get; init; } = [];
}
