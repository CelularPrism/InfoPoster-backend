using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Tools;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
            await _context.Posters.AsNoTracking().ToListAsync();

        public async Task<List<PosterResponseModel>> GetListNoTracking(DateTime start, DateTime end, string lang = "en") =>
            await _context.Posters.Join(_context.Categories,
                                        p => p.CategoryId,
                                        c => c.Id,
                                        (p, c) => p)
                                  .Join(_context.PostersMultilang,
                                        p => p.Id,
                                        m => m.PosterId,
                                        (p, m) => new { p, m })
                                  .Where(p => p.m.Lang == lang)
                                  .Select(p => new PosterResponseModel(p.p, p.m))
                                  .OrderBy(p => p.ReleaseDate)
                                  .AsNoTracking()
                                  .ToListAsync();

        public async Task<List<PosterResponseModel>> GetListNoTracking(DateTime start, DateTime end, Guid categoryId, string lang = "en") =>
            await _context.Posters.Where(p => p.CategoryId == categoryId)
                                  .Join(_context.Categories,
                                        p => p.CategoryId,
                                        c => c.Id,
                                        (p, c) => p)
                                  .Join(_context.PostersMultilang,
                                        p => p.Id,
                                        m => m.PosterId,
                                        (p, m) => new { p, m })
                                  .Where(p => p.m.Lang == lang)
                                  .Select(p => new PosterResponseModel(p.p, p.m))
                                  .OrderBy(p => p.ReleaseDate)
                                  .AsNoTracking()
                                  .ToListAsync();

        public async Task<List<PosterResponseModel>> GetListBySubcategoryNoTracking(DateTime start, DateTime end, Guid subcategoryId, string lang = "en") =>
            await _context.Posters.Join(_context.PosterSubcategory,
                                        p => p.Id,
                                        c => c.PosterId,
                                        (p, c) => new { p, c.SubcategoryId })
                                  .Where(c => c.SubcategoryId == subcategoryId)
                                  .Join(_context.PostersMultilang,
                                        p => p.p.Id,
                                        m => m.PosterId,
                                        (p, m) => new { p, m })
                                  .Where(p => p.m.Lang == lang)
                                  .Select(p => new PosterResponseModel(p.p.p, p.m))
                                  .OrderBy(p => p.ReleaseDate)
                                  .AsNoTracking()
                                  .ToListAsync();

        public async Task<PosterFullInfoResponseModel> GetFullInfo(Guid Id, string lang = "en")
        {
            var poster = await _context.Posters.Where(p => p.Id == Id)
                                               .Join(_context.Categories,
                                                    p => p.CategoryId,
                                                    c => c.Id,
                                                    (p, c) => p)
                                               .Join(_context.PostersMultilang,
                                                    p => p.Id,
                                                    m => m.PosterId,
                                                    (p, m) => new { p, m })
                                               .Where(p => p.m.Lang == lang)
                                               .Select(p => new PosterFullInfoResponseModel(p.p, p.m))
                                               .OrderBy(p => p.ReleaseDate)
                                               .AsNoTracking()
                                               .FirstOrDefaultAsync();

            poster.GaleryUrls.Add("https://a-a-ah-ru.s3.amazonaws.com/uploads/items/137024/280166/large_24_1024.jpg");
            poster.GaleryUrls.Add("https://freshmus.ru/wp-content/uploads/2023/09/Pervye-proby-v-muzyke-1-e1693768780577.jpg");
            return poster;
        }

        public async Task AddPoster(PosterModel model)
        {
            await _context.Posters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        //public async Task AddPosterFullInfo(PosterFullInfoModel model)
        //{
            
        //}

        public async Task AddPosterMultilang(PosterMultilangModel model)
        {
            await _context.PostersMultilang.AddAsync(model);
            await _context.SaveChangesAsync();
        }
    }
}
