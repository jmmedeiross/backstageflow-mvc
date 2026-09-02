using BackstageFlow.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BackstageFlow.Web.Data;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(AppDbContext dbContext)
    {
        await dbContext.Database.EnsureCreatedAsync();
        if (await dbContext.Events.AnyAsync())
        {
            return;
        }

        DateTime eventDay = DateTime.Today.AddDays(14).AddHours(16);
        var festival = new Event
        {
            Name = "Festival Horizonte",
            Venue = "Armazém Central",
            City = "São Paulo",
            StartsAt = eventDay,
            EndsAt = eventDay.AddHours(10),
            Status = EventStatus.Confirmed,
            Notes = "Evento fictício criado para demonstração do portfólio."
        };

        var session = new Event
        {
            Name = "Sessions Aurora",
            Venue = "Estúdio Norte",
            City = "Rio de Janeiro",
            StartsAt = eventDay.AddDays(21).AddHours(2),
            EndsAt = eventDay.AddDays(21).AddHours(7),
            Status = EventStatus.Planning
        };

        dbContext.Events.AddRange(festival, session);
        await dbContext.SaveChangesAsync();

        var artists = new List<ArtistBooking>
        {
            CreateArtist(festival.Id, "Luna Prado", eventDay.AddMinutes(30), eventDay.AddHours(4), "A", ArtistStatus.Confirmed),
            CreateArtist(festival.Id, "DJ Aurora", eventDay.AddHours(1), eventDay.AddHours(5.5), "B", ArtistStatus.Confirmed),
            CreateArtist(festival.Id, "Coletivo Solar", eventDay.AddHours(1.5), eventDay.AddHours(7), "C", ArtistStatus.Invited),
            CreateArtist(session.Id, "Nina Vale", session.StartsAt.AddMinutes(30), session.StartsAt.AddHours(2), "Sala 1", ArtistStatus.Confirmed)
        };

        dbContext.Artists.AddRange(artists);
        await dbContext.SaveChangesAsync();
    }

    private static ArtistBooking CreateArtist(
        int eventId,
        string stageName,
        DateTime arrival,
        DateTime performance,
        string room,
        ArtistStatus status)
    {
        var artist = new ArtistBooking
        {
            EventId = eventId,
            StageName = stageName,
            ContactName = "Produção " + stageName,
            ContactEmail = stageName.Replace(" ", ".").ToLowerInvariant() + "@example.com",
            ArrivalTime = arrival,
            PerformanceTime = performance,
            SetDurationMinutes = 45,
            DressingRoom = room,
            HospitalityNotes = "Água, frutas e toalhas.",
            TransportDetails = "Transporte organizado pela produção."
        };
        artist.SetStatus(status);
        return artist;
    }
}
