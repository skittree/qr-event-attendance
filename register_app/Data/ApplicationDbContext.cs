using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using register_app.Data.Models;

namespace register_app.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {

        public DbSet<Event> Events { get; set; }
        public DbSet<Attendee> Attendees { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Attendee>()
               .HasOne(x => x.Event)
               .WithMany(x => x.Attendees)
               .HasForeignKey(x => x.EventId)
               .OnDelete(DeleteBehavior.Cascade);
        }
    }

}