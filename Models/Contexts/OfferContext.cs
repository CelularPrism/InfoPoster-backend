using InfoPoster_backend.Models.Account;
using InfoPoster_backend.Models.Administration;
using InfoPoster_backend.Models.Cities;
using InfoPoster_backend.Models.Offers;
using InfoPoster_backend.Models.Posters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace InfoPoster_backend.Models.Contexts
{
    public class OfferContext : DbContext
    {
        public OfferContext(DbContextOptions<OfferContext> options) : base(options)
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
            var offerTypeConverter = new EnumToNumberConverter<OFFER_TYPES, int>();
            var offerStatusConverter = new EnumToNumberConverter<POSTER_STATUS, int>();
            modelBuilder.Entity<OffersModel>(entity =>
            {
                entity.Property(prop => prop.Type).HasConversion(offerTypeConverter);
                entity.Property(prop => prop.Status).HasConversion(offerStatusConverter);
                
            });
        }

        public DbSet<CityModel> Cities { get; set; }
        public DbSet<CityMultilangModel> CitiesMultilang { get; set; }
        public DbSet<OffersModel> Offers { get; set; }
        public DbSet<OffersMultilangModel> OffersMultilang { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<PopularityModel> Popularity { get; set; }
    }
}
