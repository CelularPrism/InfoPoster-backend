using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Posters;
using Microsoft.EntityFrameworkCore;

namespace InfoPoster_backend.Repos
{
    public class CategoryRepository
    {
        private PostersContext _context;

        public CategoryRepository(PostersContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryModel>> GetCategoriesNoTracking() =>
            await _context.Categories.OrderByDescending(c => c.Name).ToListAsync();
    }
}
