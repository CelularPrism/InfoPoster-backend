using InfoPoster_backend.Models.Account;
using InfoPoster_backend.Models.Cities;
using InfoPoster_backend.Models.Posters;
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
        public DbSet<PosterContactsModel> PostersContact { get; set; }
        public DbSet<CategoryModel> Categories { get; set; }
        public DbSet<CategoryMultilangModel> CategoriesMultilang { get; set; }
        public DbSet<SubcategoryModel> Subcategories { get; set; }
        public DbSet<SubcategoryMultilangModel> SubcategoriesMultilang { get; set; }
        public DbSet<PosterSubcategoryModel> PosterSubcategory { get; set; }
        public DbSet<PosterViewLogModel> PosterViewLogs { get; set; }
        public DbSet<FileURLModel> FileUrls { get; set; }
        public DbSet<PlaceModel> Places { get; set; }
    }
}
