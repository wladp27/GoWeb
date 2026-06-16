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
    class StatusJoiningConfigure : IEntityTypeConfiguration<StatusJoining>
    {
        public void Configure(EntityTypeBuilder<StatusJoining> builder)
        {
            builder.ToTable("statuses_joining");
            builder.Property(s => s.Id).HasColumnName("id");
            builder.Property(s => s.TypeStatus)
                   .HasColumnName("type_status")
                   .HasMaxLength(20)
                   .IsRequired();
            builder.HasMany(s => s.UserEvents)
                    .WithOne(eu => eu.StatusJoining)
                    .HasForeignKey(e => e.StatusJoiningId)
                    .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
