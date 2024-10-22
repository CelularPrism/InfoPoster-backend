using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Contexts;
using Microsoft.EntityFrameworkCore;

namespace InfoPoster_backend.Repos
{
    public class CategoryRepository
    {
        private readonly PostersContext _context;

        public CategoryRepository(PostersContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryResponseModel>> GetCategoriesNoTracking(int type, string lang = "en") =>
                             await _context.Categories.Where(c => c.Type == type)
                                                      .Join(_context.CategoriesMultilang,
                                                            c => c.Id,
                                                            ml => ml.CategoryId,
                                                            (c, ml) => new { c, ml })
                                                      .Where(ml => ml.ml.lang == lang)
                                                      .Select(ml => new CategoryResponseModel() { Id = ml.c.Id, Name = ml.ml.Name, ImageSrc = ml.c.ImageSrc, Type = ml.c.Type })
                                                      .OrderByDescending(c => c.Name)
                                                      .AsNoTracking()
                                                      .ToListAsync();

        public async Task<SubcategoryResponseModel> GetSubcategoriesNoTracking(Guid categoryId, string lang = "en")
        {
            var subcategories = await _context.Subcategories.Where(c => c.CategoryId == categoryId)
                                                            .Join(_context.SubcategoriesMultilang,
                                                                  c => c.Id,
                                                                  ml => ml.SubcategoryId,
                                                                  (c, ml) => new { c, ml })
                                                            .Where(ml => ml.ml.lang == lang)
                                                            .Select(ml => new SubcategoryModel() { Id = ml.c.Id, Name = ml.ml.Name, CategoryId = ml.c.Id, ImageSrc = ml.c.ImageSrc })
                                                            .OrderByDescending(c => c.Name)
                                                            .AsNoTracking()
                                                            .ToListAsync();

            var categoryName = await _context.CategoriesMultilang.Where(c => c.CategoryId == categoryId && c.lang == lang).Select(c => c.Name).AsNoTracking().FirstOrDefaultAsync();
            var result = new SubcategoryResponseModel()
            {
                CategoryName = categoryName,
                Subcategories = subcategories
            };

            return result;
        }
    }
}
