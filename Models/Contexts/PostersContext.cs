using InfoPoster_backend.Models.Account;
using InfoPoster_backend.Models.Cities;
using InfoPoster_backend.Models.Organizations;
using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Models.Selectel;
using Microsoft.EntityFrameworkCore;

namespace InfoPoster_backend.Models.Contexts
{
    public class PostersContext : DbContext
    {
        public PostersContext(DbContextOptions<PostersContext> options) : base(options)
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

        public DbSet<UserModel> Users { get; set; }
        public DbSet<PosterModel> Posters { get; set; }
        public DbSet<PosterFullInfoModel> PostersFullInfo { get; set; }
        public DbSet<PosterMultilangModel> PostersMultilang { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<CategoryMultilangModel> CategoriesMultilang { get; set; }
        public DbSet<SubcategoryModel> Subcategories { get; set; }
        public DbSet<SubcategoryMultilangModel> SubcategoriesMultilang { get; set; }
        public DbSet<PosterSubcategoryModel> PosterSubcategory { get; set; }
        public DbSet<PosterViewLogModel> PosterViewLogs { get; set; }
        public DbSet<FileURLModel> FileUrls { get; set; }
        public DbSet<PlaceModel> Places { get; set; }
        public DbSet<SelectelFileURLModel> SelectelFileURLs { get; set; }
        public DbSet<FileToApplication> FileToApplication { get; set; }
        public DbSet<ContactModel> Contacts { get; set; }
        public DbSet<ApplicationHistoryModel> ApplicationHistory { get; set; }
        public DbSet<OrganizationModel> Organizations { get; set; }
        public DbSet<OrganizationMultilangModel> OrganizationsMultilang { get; set; }
        public DbSet<CityModel> Cities { get; set; }
    }
}
