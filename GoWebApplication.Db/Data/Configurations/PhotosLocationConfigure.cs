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
    class PhotosLocationConfigure : IEntityTypeConfiguration<PhotosLocation>
    {
        public void Configure(EntityTypeBuilder<PhotosLocation> builder)
        {

            builder.ToTable("photos_locations");
            builder.Property(p => p.Id).HasColumnName("id");

            builder.Property(p => p.PhotoPath).HasColumnName("photo_path")
                   .HasMaxLength(50)
                   .IsRequired();
            builder.Property(p => p.LocationId).HasColumnName("location_id")
                   .IsRequired();
            builder.HasOne(p => p.Location)
                   .WithMany(l => l.PhotosLocations)
                   .HasForeignKey(p => p.LocationId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
