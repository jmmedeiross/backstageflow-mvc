using System.ComponentModel.DataAnnotations;

namespace BackstageFlow.Web.Models;

public enum ArtistStatus
{
    [Display(Name = "Convidado")]
    Invited = 1,
    [Display(Name = "Confirmado")]
    Confirmed = 2,
    [Display(Name = "Presente")]
    Arrived = 3,
    [Display(Name = "Apresentando")]
    Performing = 4,
    [Display(Name = "Finalizado")]
    Completed = 5,
    [Display(Name = "Cancelado")]
    Cancelled = 6
}
