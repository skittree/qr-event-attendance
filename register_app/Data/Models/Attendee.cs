using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;


namespace register_app.Data.Models
{
    public class Attendee
    {
        [Key]

        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int? EventId { get; set; }
        public Event Event { get; set; }


    }
}
