using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Tools;
using Microsoft.EntityFrameworkCore;

namespace InfoPoster_backend.Repos
{
    public class PosterRepository
    {
        private readonly PostersContext _context;
        private readonly string _lang;

        public PosterRepository(PostersContext context, IHttpContextAccessor accessor)
        {
            _context = context;
            _lang = accessor.HttpContext.Items[Constants.HTTP_ITEM_ClientLang].ToString();
        }

        public async Task<List<PosterModel>> GetListNoTracking() =>
            await _context.Posters.OrderBy(p => p.ReleaseDate).AsNoTracking().ToListAsync();

        public async Task<List<PosterResponseModel>> GetListNoTracking(DateTime start, DateTime end) =>
            await _context.Posters.Where(p => p.ReleaseDate.Date >= start.Date
                                            && p.ReleaseDate.Date <= end.Date)
                                  .Join(_context.Categories,
                                        p => p.CategoryId,
                                        c => c.Id,
                                        (p, c) => new {p, c.Name})
                                  .Join(_context.PostersMultilang,
                                        p => p.p.Id,
                                        m => m.PosterId,
                                        (p, m) => new PosterResponseModel(p.p, m) { CategoryName = p.Name })
                                  .OrderBy(p => p.ReleaseDate)
                                  .AsNoTracking()
                                  .ToListAsync();

        public async Task<List<PosterResponseModel>> GetListNoTracking(DateTime start, DateTime end, Guid categoryId) =>
            await _context.Posters.Where(p => p.ReleaseDate.Date >= start.Date 
                                           && p.ReleaseDate.Date <= end.Date 
                                           && p.CategoryId == categoryId)
                                  .Join(_context.Categories,
                                        p => p.CategoryId,
                                        c => c.Id,
                                        (p, c) => new { p, c.Name })
                                  .Join(_context.PostersMultilang,
                                        p => p.p.Id,
                                        m => m.PosterId,
                                        (p, m) => new PosterResponseModel(p.p, m) { CategoryName = p.Name })
                                  .OrderBy(p => p.ReleaseDate)
                                  .AsNoTracking()
                                  .ToListAsync();

        public async Task<PosterFullInfoResponseModel> GetFullInfo(Guid Id)
        {
            var poster = await _context.Posters.Where(p => p.Id == Id)
                                               .Join(_context.Categories,
                                                    p => p.CategoryId,
                                                    c => c.Id,
                                                    (p, c) => new { p, c.Name })
                                               .Join(_context.PostersMultilang,
                                                    p => p.p.Id,
                                                    m => m.PosterId,
                                                    (p, m) => new PosterFullInfoResponseModel(p.p, m) { CategoryName = p.Name })
                                               .OrderBy(p => p.ReleaseDate)
                                               .AsNoTracking()
                                               .FirstOrDefaultAsync();

            poster.GaleryUrls.Add("https://a-a-ah-ru.s3.amazonaws.com/uploads/items/137024/280166/large_24_1024.jpg");
            poster.GaleryUrls.Add("https://freshmus.ru/wp-content/uploads/2023/09/Pervye-proby-v-muzyke-1-e1693768780577.jpg");
            return poster;
        }
    }
}
