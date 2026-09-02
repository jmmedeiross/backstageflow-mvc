using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BackstageFlow.Web.Models;

public sealed class ArtistBooking
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Informe o nome artístico.")]
    [StringLength(100)]
    [Display(Name = "Nome artístico")]
    public string StageName { get; set; } = string.Empty;

    [StringLength(100)]
    [Display(Name = "Nome do contato")]
    public string? ContactName { get; set; }

    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    [StringLength(160)]
    [Display(Name = "E-mail")]
    public string? ContactEmail { get; set; }

    [Phone(ErrorMessage = "Informe um telefone válido.")]
    [StringLength(30)]
    [Display(Name = "Telefone")]
    public string? Phone { get; set; }

    [Required]
    [Display(Name = "Chegada prevista")]
    public DateTime ArrivalTime { get; set; }

    [Required]
    [Display(Name = "Horário da apresentação")]
    public DateTime PerformanceTime { get; set; }

    [Range(5, 300, ErrorMessage = "A duração deve ficar entre 5 e 300 minutos.")]
    [Display(Name = "Duração do set (minutos)")]
    public int SetDurationMinutes { get; set; } = 45;

    [StringLength(40)]
    [Display(Name = "Camarim")]
    public string? DressingRoom { get; set; }

    [StringLength(1000)]
    [Display(Name = "Hospitalidade")]
    public string? HospitalityNotes { get; set; }

    [StringLength(1000)]
    [Display(Name = "Transporte")]
    public string? TransportDetails { get; set; }

    [Display(Name = "Status")]
    public ArtistStatus Status { get; private set; } = ArtistStatus.Invited;

    [Display(Name = "Check-in")]
    public DateTime? CheckedInAtUtc { get; private set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    [Required]
    [Display(Name = "Evento")]
    public int EventId { get; set; }
    [ValidateNever]
    public Event Event { get; set; } = null!;

    public void SetStatus(ArtistStatus status)
    {
        Status = status;
        if (status is ArtistStatus.Invited or ArtistStatus.Confirmed or ArtistStatus.Cancelled)
        {
            CheckedInAtUtc = null;
        }
    }

    public void CheckIn(DateTime? nowUtc = null)
    {
        if (Status is ArtistStatus.Cancelled or ArtistStatus.Completed)
        {
            throw new InvalidOperationException("Não é possível realizar check-in neste status.");
        }

        Status = ArtistStatus.Arrived;
        CheckedInAtUtc = nowUtc ?? DateTime.UtcNow;
    }
}
