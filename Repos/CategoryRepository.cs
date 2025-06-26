using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Contexts;
using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Tools;
using Microsoft.EntityFrameworkCore;

namespace InfoPoster_backend.Repos
{
    public class CategoryRepository
    {
        private readonly PostersContext _posters;
        private readonly OrganizationContext _organizations;
        private readonly string _lang;

        public CategoryRepository(PostersContext posters, OrganizationContext organizations, IHttpContextAccessor accessor)
        {
            _posters = posters;
            _organizations = organizations;
            _lang = accessor.HttpContext.Items[Constants.HTTP_ITEM_ClientLang].ToString();
        }

        public async Task<List<SubcategoryModel>> GetSubcategories(int type) =>
            await _posters.Categories.Where(c => c.Type == type).Join(_posters.Subcategories,
                                                                      c => c.Id,
                                                                      s => s.CategoryId,
                                                                      (c, s) => s).ToListAsync();

        public async Task<List<SubcategoryModel>> GetPublishedSubcategories(int type)
        {
            return await _posters.Categories.Where(c => c.Type == type).Join(_posters.Subcategories,
                                                                            c => c.Id,
                                                                            s => s.CategoryId,
                                                                            (c, s) => s).ToListAsync();
        }

        public async Task<List<CategoryModel>> SearchCategories(string searchText) => await _posters.CategoriesMultilang.Where(c => c.Name.Contains(searchText) && c.lang == _lang)
                                                                                                                        .Join(_posters.Categories,
                                                                                                                              ml => ml.CategoryId,
                                                                                                                              c => c.Id,
                                                                                                                              (ml, c) => new CategoryModel()
                                                                                                                              {
                                                                                                                                  Id = c.Id,
                                                                                                                                  ImageSrc = c.ImageSrc,
                                                                                                                                  Name = ml.Name,
                                                                                                                                  Type = c.Type
                                                                                                                              }).Take(5).AsNoTracking().ToListAsync();

        public async Task<List<SubcategoryMultilangModel>> SearchSubcategories(string searchText) => await _posters.SubcategoriesMultilang.Where(c => c.Name.Contains(searchText) && c.lang == _lang).Take(5).AsNoTracking().ToListAsync();

        public async Task<List<CategoryResponseModel>> GetCategoriesNoTracking(CategoryType type, string lang = "en", bool isAdmin = false)
        {
            var categories = new List<Guid>();
            if (isAdmin)
            {
                categories = await _posters.Categories.Where(c => c.Type == (int)type).GroupBy(p => p.Id).Select(p => p.Key).ToListAsync();
            } else
            {
                if (type == CategoryType.EVENT)
                {
                    var posters = await _posters.Posters.Where(p => p.Status == (int)POSTER_STATUS.PUBLISHED && (p.ReleaseDate >= DateTime.UtcNow || p.ReleaseDateEnd > DateTime.UtcNow)).Select(p => p.Id).ToListAsync();
                    categories = await _organizations.ApplicationCategories.Where(c => posters.Contains(c.ApplicationId)).GroupBy(c => c.CategoryId).Select(c => c.Key).ToListAsync();
                }
                else
                {
                    var orgs = await _organizations.Organizations.Where(o => o.Status == (int)POSTER_STATUS.PUBLISHED).Select(p => p.Id).ToListAsync();
                    categories = await _organizations.ApplicationCategories.Where(c => orgs.Contains(c.ApplicationId)).GroupBy(c => c.CategoryId).Select(c => c.Key).ToListAsync();
                }
            }
                                    
            var result = await _posters.Categories.Where(c => categories.Contains(c.Id))
                                     .Join(_posters.CategoriesMultilang,
                                           c => c.Id,
                                           ml => ml.CategoryId,
                                           (c, ml) => new { c, ml })
                                     .Where(ml => ml.ml.lang == lang && ml.c.Type == (int)type)
                                     .Select(ml => new CategoryResponseModel() { Id = ml.c.Id, Name = ml.ml.Name, ImageSrc = ml.c.ImageSrc, Type = ml.c.Type })
                                     .OrderByDescending(c => c.Name)
                                     .AsNoTracking()
                                     .ToListAsync();

            return result;
        }

        public async Task<SubcategoryResponseModel> GetSubcategoriesNoTracking(Guid categoryId, string lang = "en", bool isAdmin = false)
        {
            var o = new { Id = Guid.NewGuid(), Count = 1 };
            var availableSubcategories = new[] { o }.ToList();
            if (isAdmin)
            {
                availableSubcategories = await _posters.Subcategories.Where(c => c.CategoryId == categoryId).GroupBy(o => o.Id).Select(o => new { Id = o.Key, Count = o.Count() }).ToListAsync();
            } else
            {
                availableSubcategories  = await _posters.Organizations.Where(c => c.CategoryId == categoryId && c.Status == (int)POSTER_STATUS.PUBLISHED).GroupBy(o => o.SubcategoryId).Select(o => new { Id = o.Key, Count = o.Count() }).ToListAsync();
                var posterSubcategories = await _posters.Posters.Where(p => p.CategoryId == categoryId && p.Status == (int)POSTER_STATUS.PUBLISHED).GroupBy(p => p.SubcategoryId).Select(p => new { Id = p.Key != null ? (Guid)p.Key : Guid.Empty, Count = p.Count() }).ToListAsync();

                availableSubcategories.AddRange(posterSubcategories);
            }

            var subcategories = availableSubcategories.Join(_posters.Subcategories,
                                                            o => o.Id,
                                                            s => s.Id,
                                                            (o, s) => (o, s))
                                                      .Join(_posters.SubcategoriesMultilang,
                                                            c => c.s.Id,
                                                            ml => ml.SubcategoryId,
                                                            (c, ml) => new { c, ml })
                                                      .Where(ml => ml.ml.lang == lang)
                                                      .Select(ml => new SubcategoryPopularModel() { Id = ml.c.s.Id, Name = ml.ml.Name, CategoryId = ml.c.s.Id, ImageSrc = ml.c.s.ImageSrc, CountApplications = ml.c.o.Count })
                                                      .OrderByDescending(c => c.Name)
                                                      .ToList();

            var categoryName = await _posters.CategoriesMultilang.Where(c => c.CategoryId == categoryId && c.lang == lang).Select(c => c.Name).AsNoTracking().FirstOrDefaultAsync();
            var result = new SubcategoryResponseModel()
            {
                CategoryName = categoryName,
                Subcategories = subcategories
            };

            return result;
        }
    }
}
