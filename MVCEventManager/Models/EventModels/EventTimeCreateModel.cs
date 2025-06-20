using BusinessLayer;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MVCEventManager.Models.EventModels
{
    public class EventTimeCreateModel : Event
    {
        public DateTime DateOnly { get; set; }

        [Required]
        public TimeOnly TimeOnly { get; set; }
    }
}
