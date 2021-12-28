using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Photobook.Common.Models;

namespace Photobook.Data.Configurations
{
    public class ProfileConfiguration : IEntityTypeConfiguration<Profile>
    {
        public void Configure(EntityTypeBuilder<Profile> builder)
        {
            builder.ToTable("Profiles");

            builder.Property(x => x.FirstName)
                .HasColumnType("nvarchar(50)");

            builder.Property(x => x.LastName)
                .HasColumnType("nvarchar(50)");

            builder.Property(x => x.Description)
                .HasColumnType("nvarchar(max)");

            builder.HasOne(x => x.ProfileImage)
                .WithOne()
                .HasForeignKey<Profile>(x => x.ProfileImageId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.User)
                .WithOne()
                .HasForeignKey<Profile>(x => x.UserId);
        }
    }
}
