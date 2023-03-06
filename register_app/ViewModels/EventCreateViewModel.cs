using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace register_app.ViewModels
{
    public class EventCreateViewModel
    {
        [Required(ErrorMessage = "Event name cannot be empty.")]
        [Display(Name = "Event Name")]
        public string Name { get; set; }
        public IdentityUser Organiser { get; set; }
        public string Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Please Select Event Start Date & Time.")]
        [Display(Name = "Event Start")]
        public DateTime StartTime { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-ddTHH:mm}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Please Select Event End Date & Time.")]
        [Display(Name = "Event End")]
        public DateTime EndTime { get; set; }



    }
}
