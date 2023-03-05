using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace register_app.ViewModels
{
    public class EventViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<AttendeeViewModel> Attendees { get; set; }
        public IdentityUser Organiser { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

    }
}
