using GoWebApplication.Db.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoWebApplication.Db.Data.Configurations
{
    class LocationConfigure : IEntityTypeConfiguration<Location>
    {
        public void Configure(EntityTypeBuilder<Location> builder)
        {
            builder.ToTable("locations");

            builder.Property(l => l.Id).HasColumnName("id");

            builder.Property(l=>l.Address).HasColumnName("address")
                   .HasMaxLength(50)
                   .IsRequired();

            builder.HasIndex(l => new { l.Address, l.CityId })
                   .IsUnique();

            builder.Property(l => l.LocationDescription)
                   .HasColumnName("location_description")
                   .HasMaxLength(200)
                   .IsRequired(false);

            builder.Property(l => l.CityId)
                   .HasColumnName("city_id")
                   .IsRequired();

            builder.Property(l => l.LocationLatitude)
                   .HasColumnName("location_latitude")
                   .IsRequired();

            builder.Property(l => l.LocationLongitude)
                  .HasColumnName("location_longitude")
                  .IsRequired();

            builder.HasMany(l => l.PhotosLocations)
                   .WithOne(p => p.Location)
                   .HasForeignKey(p=>p.LocationId)
                   .OnDelete(DeleteBehavior.Restrict); 

            builder.HasMany(l => l.Events)
                   .WithOne(e => e.Location)
                   .HasForeignKey(e=>e.LocationId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(l => l.City)
                   .WithMany(c => c.Locations)
                   .HasForeignKey(l => l.CityId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
