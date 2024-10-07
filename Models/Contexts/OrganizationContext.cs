using InfoPoster_backend.Models.Account;
using InfoPoster_backend.Models.Cities;
using InfoPoster_backend.Models.Organizations;
using Microsoft.EntityFrameworkCore;

namespace InfoPoster_backend.Models.Contexts
{
    public class OrganizationContext : DbContext
    {
        public OrganizationContext(DbContextOptions<OrganizationContext> options) : base(options)
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

        public DbSet<OrganizationModel> Organizations { get; set; }
        public DbSet<OrganizationFullInfoModel> OrganizationsFullInfo { get; set; }
        public DbSet<OrganizationMultilangModel> OrganizationsMultilang { get; set; }
        public DbSet<OrganizationFileURLModel> OrganizationFileUrls { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<CityModel> Cities { get; set; }
        public DbSet<CityMultilangModel> CitiesMultilang { get; set; }
    }
}
