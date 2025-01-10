using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Contexts;
using Microsoft.EntityFrameworkCore;

namespace InfoPoster_backend.Repos
{
    public class CategoryRepository
    {
        private readonly PostersContext _posters;
        private readonly OrganizationContext _organizations;

        public CategoryRepository(PostersContext posters, OrganizationContext organizations)
        {
            _posters = posters;
            _organizations = organizations;
        }

        public async Task<List<CategoryResponseModel>> GetCategoriesNoTracking(CategoryType type, string lang = "en")
        {
            var categories = new List<Guid>();
            if (type == CategoryType.EVENT)
                categories = await _posters.Posters.GroupBy(p => p.CategoryId).Select(p => p.Key).ToListAsync();
            else
                categories = await _organizations.Organizations.GroupBy(p => p.CategoryId).Select(p => p.Key).ToListAsync();
                                    
            var result = await _posters.Categories.Where(c => categories.Contains(c.Id))
                                     .Join(_posters.CategoriesMultilang,
                                           c => c.Id,
                                           ml => ml.CategoryId,
                                           (c, ml) => new { c, ml })
                                     .Where(ml => ml.ml.lang == lang)
                                     .Select(ml => new CategoryResponseModel() { Id = ml.c.Id, Name = ml.ml.Name, ImageSrc = ml.c.ImageSrc, Type = ml.c.Type })
                                     .OrderByDescending(c => c.Name)
                                     .AsNoTracking()
                                     .ToListAsync();

            return result;
        }

        public async Task<SubcategoryResponseModel> GetSubcategoriesNoTracking(Guid categoryId, string lang = "en")
        {
            var subcategories = await _posters.Organizations.Where(c => c.CategoryId == categoryId).GroupBy(o => o.SubcategoryId).Select(o => o.Key)
                                                            .Join(_posters.Subcategories,
                                                                  o => o,
                                                                  s => s.Id,
                                                                  (o, s) => s)
                                                            .Join(_posters.SubcategoriesMultilang,
                                                                  c => c.Id,
                                                                  ml => ml.SubcategoryId,
                                                                  (c, ml) => new { c, ml })
                                                            .Where(ml => ml.ml.lang == lang)
                                                            .Select(ml => new SubcategoryModel() { Id = ml.c.Id, Name = ml.ml.Name, CategoryId = ml.c.Id, ImageSrc = ml.c.ImageSrc })
                                                            .OrderByDescending(c => c.Name)
                                                            .AsNoTracking()
                                                            .ToListAsync();

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
