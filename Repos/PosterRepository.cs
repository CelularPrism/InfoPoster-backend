using InfoPoster_backend.Handlers.Posters;
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

        public async Task<PosterModel> GetPoster(Guid id) =>
            await _context.Posters.FirstOrDefaultAsync(x => x.Id == id);

        public async Task<PosterFullInfoModel> GetFullInfoPoster(Guid posterId) =>
            await _context.PostersFullInfo.FirstOrDefaultAsync(x => x.PosterId == posterId);

        public async Task<PosterMultilangModel> GetMultilangPoster(Guid posterId, string lang) =>
            await _context.PostersMultilang.FirstOrDefaultAsync(x => x.PosterId == posterId && x.Lang == lang);

        public async Task<List<AdministrationGetPostersResponse>> GetListNoTracking(string lang) =>
            await _context.Posters.Join(_context.Categories,
                                        p => p.CategoryId,
                                        c => c.Id,
                                        (p, c) => p)
                                  .Join(_context.PostersMultilang,
                                        p => p.Id,
                                        m => m.PosterId,
                                        (p, m) => new { p, m })
                                  .Where(p => p.m.Lang == lang)
                                  .Join(_context.Users,
                                        p => p.p.UserId,
                                        u => u.Id,
                                        (p, u) => new { p, UserName = u.FirstName + " " + u.LastName})
                                  .Select(p => new AdministrationGetPostersResponse(p.p.p, p.p.m, p.UserName))
                                  .OrderBy(p => p.ReleaseDate)
                                  .AsNoTracking()
                                  .ToListAsync();

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
                                               .Join(_context.PostersFullInfo,
                                                     p => p.Id,
                                                     f => f.PosterId,
                                                     (p, f) => new { p, f })
                                               .Join(_context.PostersMultilang,
                                                    p => p.f.PosterId,
                                                    m => m.PosterId,
                                                    (p, m) => new { p, m })
                                               .Where(p => p.m.Lang == lang)
                                               .Select(p => new PosterFullInfoResponseModel(p.p.p, p.p.f, p.m))
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

        public async Task AddPosterFullInfo(PosterFullInfoModel model)
        {
            await _context.PostersFullInfo.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task AddPosterMultilang(PosterMultilangModel model)
        {
            await _context.PostersMultilang.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePoster(PosterModel model)
        {
            _context.Posters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePosterFullInfo(PosterFullInfoModel model)
        {
            _context.PostersFullInfo.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePosterMultilang(PosterMultilangModel model)
        {
            _context.PostersMultilang.Update(model);
            await _context.SaveChangesAsync();
        }
    }
}
