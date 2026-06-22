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
    class RatingConfigure : IEntityTypeConfiguration<Rating>
    {
        public void Configure(EntityTypeBuilder<Rating> builder)
        {
            builder.ToTable("ratings");
            builder.HasKey(r=>new {r.UserId,r.EventTypeId});


          

            builder.Property(c => c.Value).HasColumnName("value")
                    .IsRequired();

            builder.ToTable(t => t.HasCheckConstraint(
              "CK_Rating_Value_Range", 
             "`value` >= 0 AND `value` <= 100" 
          ));

            builder.Property(c => c.EventTypeId)
                    .HasColumnName("event_type_id")
                    .IsRequired();

            builder.Property(c => c.UserId)
                    .HasColumnName("user_id")
                    .IsRequired();

            builder.HasOne(r => r.User)
                   .WithMany(u => u.Ratings)
                   .HasForeignKey(r => r.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.EventType)
                   .WithMany(e => e.Ratings)
                   .HasForeignKey(r => r.EventTypeId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
