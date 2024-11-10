using InfoPoster_backend.Models.Account;
using InfoPoster_backend.Models.Cities;
using InfoPoster_backend.Models.Organizations;
using InfoPoster_backend.Models.Organizations.Menu;
using InfoPoster_backend.Models.Selectel;
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
        public DbSet<PlaceModel> Places { get; set; }
        public DbSet<SelectelFileURLModel> SelectelFileURLs { get; set; }
        public DbSet<FileToApplication> FileToApplication { get; set; }
        public DbSet<MenuModel> Menus { get; set; }
        public DbSet<MenuToOrganizationModel> MenusToOrganization { get; set; }
        public DbSet<MenuMultilangModel> MenusMultilang { get; set; }
        public DbSet<ContactModel> Contacts { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<SubcategoryModel> Subcategories { get; set; }
        public DbSet<CategoryMultilangModel> CategoriesMultilang { get; set; }
        public DbSet<SubcategoryMultilangModel> SubcategoriesMultilang { get; set; }
        public DbSet<FileURLModel> FileUrls { get; set; }
        public DbSet<ApplicationHistoryModel> ApplicationHistory { get; set; }
    }
}
