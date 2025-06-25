using BusinessLayer;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace MVCEventManager.Models.EventModels
{
    public class EventTimeCreateModel : Event
    {
        public DateTime DateOnly { get; set; }

        [Required]
        public TimeOnly TimeOnly { get; set; }

        public bool IsDateAndTimeValid { get { return ValidateDateAndTime(); } }

        public string DateAndTimeValidatedText = "";

        private bool ValidateDateAndTime()
        {
            if (DateOnly.Date != DateTime.Now.Date)
            {
                return true;
            }

            DateTime dt = DateTime.Now.Date;
            dt = dt.Date.Add(TimeOnly.ToTimeSpan());
            if (dt < DateTime.Now)
            {
                return false;
            }

            return true;
        }
    }
}
