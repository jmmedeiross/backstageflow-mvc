using BackstageFlow.Web.Models;
using Xunit;

namespace BackstageFlow.Tests;

public sealed class ArtistBookingTests
{
    [Fact]
    public void CheckIn_WhenArtistIsConfirmed_RegistersArrival()
    {
        var artist = CreateArtist();
        DateTime nowUtc = new(2026, 9, 2, 14, 30, 0, DateTimeKind.Utc);
        artist.SetStatus(ArtistStatus.Confirmed);

        artist.CheckIn(nowUtc);

        Assert.Equal(ArtistStatus.Arrived, artist.Status);
        Assert.Equal(nowUtc, artist.CheckedInAtUtc);
    }

    [Fact]
    public void CheckIn_WhenArtistIsCancelled_ThrowsException()
    {
        var artist = CreateArtist();
        artist.SetStatus(ArtistStatus.Cancelled);

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => artist.CheckIn());

        Assert.Equal("Não é possível realizar check-in neste status.", exception.Message);
        Assert.Null(artist.CheckedInAtUtc);
    }

    [Fact]
    public void SetStatus_WhenPerformanceStarts_PreservesCheckInHistory()
    {
        var artist = CreateArtist();
        DateTime checkInUtc = new(2026, 9, 2, 14, 30, 0, DateTimeKind.Utc);
        artist.CheckIn(checkInUtc);

        artist.SetStatus(ArtistStatus.Performing);

        Assert.Equal(ArtistStatus.Performing, artist.Status);
        Assert.Equal(checkInUtc, artist.CheckedInAtUtc);
    }

    [Theory]
    [InlineData(ArtistStatus.Cancelled)]
    [InlineData(ArtistStatus.Completed)]
    public void CheckIn_WhenStatusBlocksArrival_DoesNotChangeStatus(ArtistStatus status)
    {
        var artist = CreateArtist();
        artist.SetStatus(status);

        Assert.Throws<InvalidOperationException>(() => artist.CheckIn());

        Assert.Equal(status, artist.Status);
    }

    private static ArtistBooking CreateArtist() => new()
    {
        StageName = "Artista Teste",
        EventId = 1,
        ArrivalTime = new DateTime(2026, 9, 2, 17, 0, 0),
        PerformanceTime = new DateTime(2026, 9, 2, 20, 0, 0)
    };
}
