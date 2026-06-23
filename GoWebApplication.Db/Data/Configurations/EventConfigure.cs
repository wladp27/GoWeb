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
    class EventConfigure : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.ToTable("Events");

            builder.ToTable(t => t.HasCheckConstraint(
           "limit_count_days",
           "count_days_recreate > 0"));

            builder.Property(c => c.Id)
                    .HasColumnName("id");

            builder.Property(ev => ev.EventName)
                   .HasColumnName("event_name")
                   .IsRequired();

            builder.Property(ev => ev.EventTypeId)
                    .HasColumnName("event_type_id")
                    .IsRequired(false);

            builder.Property(ev => ev.RequiredRating)
                      .HasColumnName("required_rating")
                      .HasDefaultValue(0)
                      .IsRequired();

            builder.ToTable(t => t.HasCheckConstraint(
             "CK_Rating_Value_Range",
            "`required_rating` >= 0 AND `required_rating` <= 100"
            ));
       

            builder.Property(ev => ev.StatusEventId)
                    .HasColumnName("status_event_id")
                    .IsRequired();

            builder.Property(ev => ev.OrganizerId)
                    .HasColumnName("organizer_id")
                    .IsRequired(false);

          
       

            builder.Property(ev => ev.StartTime)
                    .HasColumnName("start_time")
                    .IsRequired();

            builder.Property(ev => ev.EndTime)
                    .HasColumnName("end_time")
                    .IsRequired();

            builder.Property(ev => ev.EventDescription)
                   .HasColumnName("event_description")
                   .HasMaxLength(4000)
                   .IsRequired(false);

            builder.Property(ev => ev.ImagePath)
                   .HasColumnName("image_path")
                   .HasMaxLength(500)
                   .IsRequired();

            builder.Property(ev => ev.MaxParticipants)
                   .HasColumnName("max_participants")
                   .IsRequired();

            builder.Property(ev => ev.MinParticipants)
                   .HasColumnName("min_participants")
                   .IsRequired();

            builder.Property(ev => ev.Price)
                   .HasColumnName("price")
                   .IsRequired();

     

            builder.Property(ev=>ev.CountDaysRecreate)
                   .HasColumnName("count_days_recreate")
                   .IsRequired(false);

            builder.Property(ev => ev.LocationId)
                   .HasColumnName("location_id")
                   .IsRequired(false);
        }
    }
}
