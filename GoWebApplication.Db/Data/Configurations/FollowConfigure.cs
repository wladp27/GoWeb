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
    class FollowConfigure : IEntityTypeConfiguration<Follow>
    {
        public void Configure(EntityTypeBuilder<Follow> builder)
        {
            builder.ToTable("follows");

            builder.HasKey(f => new {f.FollowingUserId,f.FollowedUserId});

            builder.Property(f => f.FollowedUserId)
                .HasColumnName("followed_user_id")
                .HasMaxLength(50)
                .IsRequired();
            builder.Property(f => f.FollowingUserId)
                .HasColumnName("following_user_id")
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(f => f.CreatedAt)
                .HasColumnName("created_at")
                .IsRequired();


            builder.HasOne(f => f.FollowingUser)
                   .WithMany(u => u.Following)
                   .HasForeignKey(f => f.FollowingUserId);

            builder.HasOne(f => f.FollowedUser)
                   .WithMany(u => u.Followers)
                   .HasForeignKey(f => f.FollowedUserId);

        }
    }
}
