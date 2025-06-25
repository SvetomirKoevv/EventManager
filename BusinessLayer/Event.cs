using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class Event
    {
        [Key]
        public int Id { get; set; }


        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "datetime2")]
        [FutureDate(ErrorMessage = "Event start must be in the future!")]
        public DateTime EventStart { get; set; }

        [Required]
        public string Location { get; set; }

        public int? MaxAttendees { get; set; }

        public User Creator { get; set; }


        public Event()
        {

        }

        public Event(string name_, string description_, DateTime eventStart_, string location_, int maxAttendees)
        {
            this.Name = name_;
            this.Description = description_;
            this.EventStart = eventStart_;
            this.Location = location_;
            this.MaxAttendees = maxAttendees;
        }
    }
}
