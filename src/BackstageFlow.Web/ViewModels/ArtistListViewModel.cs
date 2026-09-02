using BackstageFlow.Web.Models;

namespace BackstageFlow.Web.ViewModels;

public sealed class ArtistListViewModel
{
    public IReadOnlyList<ArtistBooking> Artists { get; init; } = [];
    public IReadOnlyList<Event> Events { get; init; } = [];
    public int? EventId { get; init; }
    public ArtistStatus? Status { get; init; }
    public string? Search { get; init; }
}
