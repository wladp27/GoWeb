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
    class EventTypeConfigure : IEntityTypeConfiguration<EventType>
    {
        public void Configure(EntityTypeBuilder<EventType> builder)
        {
            builder.ToTable("event_types");
            builder.Property(ev => ev.Name)
                .HasColumnName("name")
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(ev => ev.Name)
                   .IsUnique();

            builder.Property(ev => ev.Id)
                .HasColumnName("id");
            builder.Property(ev=>ev.ImagePath)
                    .HasColumnName("image_path")
                    .HasMaxLength(100)
                    .IsRequired();

           

            builder.HasMany(ev=>ev.Events)
                   .WithOne(ev=>ev.EventType)
                   .HasForeignKey(ev=>ev.EventTypeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e=>e.Ratings)
                   .WithOne(r=>r.EventType)
                   .HasForeignKey(r=>r.EventTypeId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
