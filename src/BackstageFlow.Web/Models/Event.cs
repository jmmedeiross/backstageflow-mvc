using System.ComponentModel.DataAnnotations;

namespace BackstageFlow.Web.Models;

public sealed class Event
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o nome do evento.")]
    [StringLength(120)]
    [Display(Name = "Nome do evento")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o local.")]
    [StringLength(120)]
    [Display(Name = "Local")]
    public string Venue { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a cidade.")]
    [StringLength(80)]
    [Display(Name = "Cidade")]
    public string City { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Início")]
    public DateTime StartsAt { get; set; }

    [Required]
    [Display(Name = "Término")]
    public DateTime EndsAt { get; set; }

    [Display(Name = "Status")]
    public EventStatus Status { get; set; } = EventStatus.Planning;

    [StringLength(1000)]
    [Display(Name = "Observações")]
    public string? Notes { get; set; }

    public ICollection<ArtistBooking> Artists { get; set; } = [];
}
