using ArticlesProject.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ArticlesProject.Configurations.EntityConfigurations
{
    public class ArticleEntityConfiguration : IEntityTypeConfiguration<Article>
    {
        public void Configure(EntityTypeBuilder<Article> builder)
        {
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id).ValueGeneratedOnAdd();
            builder.Property(e => e.Title).HasMaxLength(100).IsRequired(true);
            builder.Property(e => e.Content).IsRequired(true);
            builder.Property(e => e.ImagePath).IsRequired(true);
            builder.Property(e => e.AddedOn).HasDefaultValueSql("GETDATE()");


            builder.HasOne(e => e.Category)
                .WithMany(e => e.Articles)
                .HasForeignKey(e => e.CategoryId);

            builder.HasOne(e => e.ApplicationUser)
                .WithMany(e => e.Articles)
                .HasForeignKey(e => e.ApplicationUserId);
        }
    }
}