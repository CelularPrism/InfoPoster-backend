using InfoPoster_backend.Models.Administration;
using InfoPoster_backend.Models.Banner;
using InfoPoster_backend.Models.Organizations;
using InfoPoster_backend.Models.Posters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace InfoPoster_backend.Models.Contexts
{
    public class BannerContext : DbContext
    {
        public BannerContext(DbContextOptions<BannerContext> options) : base(options)
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
            var typeConverter = new EnumToNumberConverter<POPULARITY_TYPE, int>();
            var placeConverter = new EnumToNumberConverter<POPULARITY_PLACE, int>();
            modelBuilder.Entity<PopularityModel>(entity =>
            {
                entity.Property(prop => prop.Type).HasConversion(typeConverter);
                entity.Property(prop => prop.Place).HasConversion(placeConverter);
            });
        }

        public DbSet<BannerModel> Banners { get; set; }
        public DbSet<PopularityModel> Popularity { get; set; }
        public DbSet<OrganizationModel> Organizations { get; set; }
        public DbSet<PosterModel> Posters { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<SubcategoryModel> Subcategories { get; set; }
    }
}
