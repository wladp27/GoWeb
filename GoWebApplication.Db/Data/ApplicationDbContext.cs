using GoWebApplication.Db.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GoWebApplication.Db.Data
{
    public class ApplicationDbContext: IdentityDbContext<User, IdentityRole, string>
    {
        public DbSet<Event> Events { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<EventType> EventTypes { get; set; }
        public DbSet<StatusEvent> StatEvents { get; set; }
        public DbSet<StatusJoining> StatusJoinings { get; set; }
        public DbSet<UserEvent> UsersEvents { get; set; }
        public DbSet<Location> Locations { get; set; }

        public DbSet<PhotosLocation> PhotosLocations { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
