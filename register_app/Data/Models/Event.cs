using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace register_app.Data.Models
{
    public class Event
    {

        [Key]

        public int Id { get; set; }
        public string Name { get; set; }

        public IEnumerable<Attendee> Attendees { get; set; }
        public IdentityUser Organiser { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
