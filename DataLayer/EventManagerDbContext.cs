using BusinessLayer;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer
{
    public class EventManagerDbContext : IdentityDbContext<User>
    {

        public static string ConnectionString = "Server=DESKTOP-RD8LV0K;Database=EventManager;Trusted_Connection=True;Encrypt=False";
        public EventManagerDbContext() : base() { }

        public EventManagerDbContext(DbContextOptions options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Event>()
                .Property(e => e.MaxAttendees)
                .IsRequired(false);
        }

        public DbSet<Event> Events { get; set; }
    }
}
