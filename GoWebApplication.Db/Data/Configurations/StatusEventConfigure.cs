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
    public class StatusEventConfigure: IEntityTypeConfiguration<StatusEvent>
    {
        
        public void Configure(EntityTypeBuilder<StatusEvent> builder)
        {
           builder.ToTable("stats_events");
           builder.Property(s => s.TypeStatus)
                   .HasColumnName("type_status")
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(s => s.Code)
                   .HasColumnName("code")
                   .HasMaxLength(20)
                   .IsRequired();

            builder.Property(s=>s.Id).HasColumnName("id");
           builder.HasMany(s => s.Events)
                   .WithOne(su => su.Status)
                   .HasForeignKey(v=>v.StatusEventId)
                   .OnDelete(DeleteBehavior.Restrict);


            builder.HasData(
                     new StatusEvent { Id = 1, TypeStatus = "В работе", Code = "IN_PROGRESS" },
                     new StatusEvent { Id = 2, TypeStatus = "Завершен", Code = "COMPLETED" },
                     new StatusEvent { Id = 3, TypeStatus = "Создано", Code = "CREATED" },
                     new StatusEvent { Id = 4, TypeStatus = "Отменено", Code = "Cancelled" }
              );
        }
    }
}
