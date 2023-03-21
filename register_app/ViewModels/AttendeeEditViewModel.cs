using System;
using System.ComponentModel.DataAnnotations;

namespace register_app.ViewModels
{
    public class AttendeeEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Attendee name cannot be empty.")]
        [Display(Name = "Attendee Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Attendee Email cannot be empty.")]
        [Display(Name = "Attendee Email")]
        public string Email { get; set; }
        public int? EventId { get; set; }
        public EventViewModel Event { get; set; }

        public string Key { get; set; }

        public DateTime TimeArrived { get; set; }

    }
}
