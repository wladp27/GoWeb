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
    class UserEventConfigure : IEntityTypeConfiguration<UserEvent>
    {
        public void Configure(EntityTypeBuilder<UserEvent> builder)
        {
            builder.ToTable("users_events");

            builder.HasKey(uv => new { uv.EventId, uv.UserId });

            builder.Property(uv => uv.UserId)
                .HasColumnName("user_id");

            builder.Property(uv => uv.EventId)
                .HasColumnName("event_id");

            builder.Property(uv=> uv.TimeJoinEvent)
                    .HasColumnName("time_join_event")
                    .IsRequired();

            builder.HasOne(uv => uv.User)
                .WithMany(u => u.UserEvents)
                .HasForeignKey(uv => uv.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(uv => uv.Event)
               .WithMany(u => u.UserEvents)
               .HasForeignKey(uv => uv.EventId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(uv => uv.StatusJoining)
                   .WithMany(s => s.UserEvents)
                   .HasForeignKey(uv => uv.StatusJoiningId)
                   .OnDelete(DeleteBehavior.Restrict);


    }
    }
}
