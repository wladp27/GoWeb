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
    public class CityConfigure : IEntityTypeConfiguration<City>
    {
        public void Configure(EntityTypeBuilder<City> builder)
        {
            builder.ToTable("cities");
            builder.Property(c=>c.NameCity).HasColumnName("name_city")
                    .HasMaxLength(50)
                    .IsRequired();
            builder.HasIndex(ev => ev.NameCity)
                  .IsUnique();
            builder.Property(c => c.Id)
                    .HasColumnName("id");
            builder.Property(c => c.LocationLatitude)
                    .HasColumnName("location_latitude")
                    .IsRequired();
            builder.Property(c => c.LocationLongitude)
                    .HasColumnName("location_longitude")
                    .IsRequired();

      

            builder.HasMany(c => c.Users)
                    .WithOne(u=>u.City)
                    .HasForeignKey(c=>c.idCity)
                    .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
