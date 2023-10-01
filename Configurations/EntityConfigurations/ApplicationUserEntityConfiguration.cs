using ArticlesProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArticlesProject.Configurations.EntityConfigurations
{
    public class ApplicationUserEntityConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(e => e.FirstName).HasMaxLength(50).IsRequired(true);
            builder.Property(e => e.LastName).HasMaxLength(50).IsRequired(true);
            builder.Property(e => e.ProfilePicturePath).IsRequired(false);
            builder.Property(e => e.Bio).IsRequired(true);
            builder.Property(e => e.Facebook).IsRequired(true);
            builder.Property(e => e.Instagram).IsRequired(false);
            builder.Property(e => e.Twiter).IsRequired(false);
        }
    }
}