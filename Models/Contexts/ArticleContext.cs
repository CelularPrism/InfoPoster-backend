using InfoPoster_backend.Models.Account;
using InfoPoster_backend.Models.Administration;
using InfoPoster_backend.Models.Posters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace InfoPoster_backend.Models.Contexts
{
    public class ArticleContext : DbContext
    {
        public ArticleContext(DbContextOptions<ArticleContext> options) : base(options)
        {
            try
            {
                Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                var exc = ex;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var articleStatusConverter = new EnumToNumberConverter<POSTER_STATUS, int>();
            modelBuilder.Entity<ArticleModel>(entity =>
            {
                entity.Property(prop => prop.Status).HasConversion(articleStatusConverter);

            });
        }

        public DbSet<UserModel> Users { get; set; }
        public DbSet<ArticleModel> Articles { get; set; }
        public DbSet<PopularityModel> Popularity { get; set; }
    }
}
