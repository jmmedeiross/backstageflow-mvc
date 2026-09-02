using System.ComponentModel.DataAnnotations;

namespace BackstageFlow.Web.Models;

public enum EventStatus
{
    [Display(Name = "Planejamento")]
    Planning = 1,
    [Display(Name = "Confirmado")]
    Confirmed = 2,
    [Display(Name = "Ao vivo")]
    Live = 3,
    [Display(Name = "Concluído")]
    Completed = 4
}
