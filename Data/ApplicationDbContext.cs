using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ArticlesProject.Models;
using ArticlesProject.Configurations.EntityConfigurations;
using ArticlesProject.ViewModels;

namespace ArticlesProject.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");


            builder.ApplyConfigurationsFromAssembly(typeof(CategoryEntityConfiguration).Assembly);
            builder.ApplyConfigurationsFromAssembly(typeof(ArticleEntityConfiguration).Assembly);
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<ArticlesProject.ViewModels.ArticleDetailesViewModel> ArticleDetailesViewModel { get; set; } = default!;
        public DbSet<ArticlesProject.ViewModels.PublisherViewModel> PublisherViewModel { get; set; } = default!;
    }
}