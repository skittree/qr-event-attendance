using register_app.Data.Models;
using System;

namespace register_app.ViewModels
{
    public class AttendeeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int? EventId { get; set; }
        public EventViewModel Event { get; set; }

        public DateTime TimeArrived { get; set; }
        public string Key { get; set; }

    }
}
