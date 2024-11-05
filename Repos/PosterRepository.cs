using InfoPoster_backend.Handlers.Administration;
using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Cities;
using InfoPoster_backend.Models.Contexts;
using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Models.Selectel;
using InfoPoster_backend.Tools;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;
using System.Linq;

namespace InfoPoster_backend.Repos
{
    public class PosterRepository
    {
        private readonly PostersContext _context;
        private readonly string _lang;
        private readonly Guid _city;

        public PosterRepository(PostersContext context, IHttpContextAccessor accessor)
        {
            _context = context;
            _lang = accessor.HttpContext.Items[Constants.HTTP_ITEM_ClientLang].ToString();
            _city = Guid.TryParse(accessor.HttpContext.Request.Headers["X-Testing"].ToString(), out _city) ? Guid.Parse(accessor.HttpContext.Request.Headers["X-Testing"].ToString()) : Constants.DefaultCity;
        }

        public async Task<PosterModel> GetPoster(Guid id) =>
            await _context.Posters.FirstOrDefaultAsync(x => x.Id == id);

        public async Task<PosterFullInfoModel> GetFullInfoPoster(Guid posterId) =>
            await _context.PostersFullInfo.FirstOrDefaultAsync(x => x.PosterId == posterId);

        public async Task<PosterMultilangModel> GetMultilangPoster(Guid posterId, string lang) =>
            await _context.PostersMultilang.FirstOrDefaultAsync(x => x.PosterId == posterId && x.Lang == lang);

        public async Task<List<PosterMultilangModel>> GetMultilangPosterList(Guid posterId) =>
            await _context.PostersMultilang.Where(x => x.PosterId == posterId).ToListAsync();

        public async Task<ContactModel> GetContact(Guid posterId) =>
            await _context.Contacts.FirstOrDefaultAsync(c => c.ApplicationId == posterId);

        public async Task<List<FileURLModel>> GetFileUrls(Guid posterId) =>
            await _context.FileUrls.Where(f => f.PosterId == posterId).ToListAsync();

        public async Task<List<PlaceModel>> GetPlaces(Guid organizationId) =>
            await _context.Places.Where(p => p.ApplicationId == organizationId && p.Lang == _lang).ToListAsync();

        public async Task<SelectelFileURLModel> GetSelectelFile(Guid organizationId) =>
            await _context.FileToApplication.Where(f => f.ApplicationId == organizationId)
                                                 .Join(_context.SelectelFileURLs,
                                                       f => f.FileId,
                                                       s => s.Id,
                                                       (f, s) => s)
                                                 .FirstOrDefaultAsync();

        public async Task<List<GetAllPostersResponse>> GetListNoTracking(string lang)
        {
            var list = await _context.Posters.Where(p => p.Status == (int)POSTER_STATUS.PENDING || p.Status == (int)POSTER_STATUS.PUBLISHED)
                                  .Join(_context.Categories,
                                        p => p.CategoryId,
                                        c => c.Id,
                                        (p, c) => p)
                                  .Join(_context.PostersMultilang,
                                        p => p.Id,
                                        m => m.PosterId,
                                        (p, m) => new { p, m })
                                  .Where(p => p.m.Lang == "en")
                                  .Join(_context.Users,
                                        p => p.p.UserId,
                                        u => u.Id,
                                        (p, u) => new { p, UserName = u.FirstName + " " + u.LastName })
                                  .AsNoTracking()
                                  .ToListAsync();

            var result = list.Select(p => new GetAllPostersResponse(p.p.p, p.p.m, p.UserName))
                             .OrderBy(p => p.ReleaseDate)
                             .ToList();
            return result;
        }

        public async Task<List<AdministrationGetPostersResponse>> GetListNoTracking(Guid userId, string lang)
        {
            var list = await _context.Posters.Where(p => p.UserId == userId)
                                  .Join(_context.PostersMultilang,
                                        p => p.Id,
                                        m => m.PosterId,
                                        (p, m) => new { p, m })
                                  .Where(p => p.m.Lang == "en")
                                  .Join(_context.Users,
                                        p => p.p.UserId,
                                        u => u.Id,
                                        (p, u) => new { p, UserName = u.FirstName + " " + u.LastName })
                                  .AsNoTracking()
                                  .ToListAsync();

            var result = list.Select(p => new AdministrationGetPostersResponse(p.p.p, p.p.m, p.UserName))
                             .OrderBy(p => p.ReleaseDate)
                             .ToList();
            return result;
        }

        public async Task<List<PosterResponseModel>> GetListNoTracking(DateTime start, DateTime end, string lang = "en")
        {
            var list = await _context.Posters
                                  .Join(_context.PostersFullInfo,
                                        p => p.Id,
                                        f => f.PosterId,
                                        (p, f) => new { p, f })
                                  .Where(p => p.f.City == _city)
                                  .Join(_context.PostersMultilang,
                                        p => p.p.Id,
                                        m => m.PosterId,
                                        (p, m) => new { p, m })
                                  .Where(p => p.m.Lang == lang && p.p.p.Status == (int)POSTER_STATUS.PUBLISHED)
                                  .AsNoTracking()
                                  .ToListAsync();

            var result = list.Select(p => new PosterResponseModel(p.p.p, p.m))
                             .OrderBy(p => p.ReleaseDate)
                             .ToList();
            return result;
        }

        public async Task<List<PosterResponseModel>> GetListNoTracking(DateTime start, DateTime end, Guid categoryId, string lang = "en")
        {
            var list = await _context.Posters.Where(p => p.CategoryId == categoryId)
                                  .Join(_context.PostersFullInfo,
                                        p => p.Id,
                                        f => f.PosterId,
                                        (p, f) => new { p, f })
                                  .Where(p => p.f.City == _city)
                                  .Join(_context.PostersMultilang,
                                        p => p.p.Id,
                                        m => m.PosterId,
                                        (p, m) => new { p, m })
                                  .Where(p => p.m.Lang == lang && p.p.p.Status == (int)POSTER_STATUS.PUBLISHED)
                                  .AsNoTracking()
                                  .ToListAsync();

            var categories = await _context.CategoriesMultilang.Where(c => c.lang == lang).ToListAsync();

            var result = list.Select(p => new PosterResponseModel(p.p.p, p.m) 
                                            { 
                                                CategoryName = categories.Where(c => c.CategoryId == p.p.p.CategoryId).Select(c => c.Name).FirstOrDefault(), 
                                                FileId = _context.FileToApplication.Where(f => f.ApplicationId == p.p.p.Id && f.IsPrimary).Select(f => f.FileId).FirstOrDefault(),
                                            })
                             .OrderBy(p => p.ReleaseDate)
                             .ToList();
            return result;
        }

        public async Task<List<PosterResponseModel>> GetListBySubcategoryNoTracking(DateTime start, DateTime end, Guid subcategoryId, string lang = "en")
        {
            var list = await _context.Posters.Join(_context.PosterSubcategory,
                                        p => p.Id,
                                        c => c.PosterId,
                                        (p, c) => new { p, c.SubcategoryId })
                                  .Where(c => c.SubcategoryId == subcategoryId)
                                  .Join(_context.PostersMultilang,
                                        p => p.p.Id,
                                        m => m.PosterId,
                                        (p, m) => new { p, m })
                                  .Where(p => p.m.Lang == lang && p.p.p.Status == (int)POSTER_STATUS.PUBLISHED)
                                  .AsNoTracking()
                                  .ToListAsync();

            var result = list.Select(p => new PosterResponseModel(p.p.p, p.m))
                             .OrderBy(p => p.ReleaseDate)
                             .ToList();
            return result;
        }

        public async Task<PosterFullInfoResponseModel> GetFullInfo(Guid Id, string lang = "en")
        {
            var model = await _context.Posters.Where(p => p.Id == Id)
                                               .Join(_context.PostersFullInfo,
                                                     p => p.Id,
                                                     f => f.PosterId,
                                                     (p, f) => new { p, f })
                                               .Join(_context.PostersMultilang,
                                                    p => p.f.PosterId,
                                                    m => m.PosterId,
                                                    (p, m) => new { p, m })
                                               .Where(p => p.m.Lang == lang)
                                               .Select(p => new { Poster = p.p.p, FullInfo = p.p.f, Multilang = p.m })
                                               .AsNoTracking()
                                               .FirstOrDefaultAsync();

            var poster = new PosterFullInfoResponseModel(model.Poster, model.FullInfo, model.Multilang);
            poster.CategoryName = await _context.CategoriesMultilang.Where(c => c.CategoryId == poster.CategoryId && c.lang == lang)
                                                                    .AsNoTracking()
                                                                    .Select(c => c.Name)
                                                                    .FirstOrDefaultAsync();
            var files = await _context.FileUrls.Where(f => f.PosterId == Id)
                                               .AsNoTracking()
                                               .ToListAsync();

            poster.SocialLinks = files.Where(f => f.FileCategory == (int)FILE_CATEGORIES.SOCIAL_LINKS).Select(f => f.URL).ToList();
            poster.VideoUrls = files.Where(f => f.FileCategory == (int)FILE_CATEGORIES.VIDEO).Select(f => f.URL).ToList();

            var places = await _context.Places.Where(p => p.ApplicationId == Id)
                                              .AsNoTracking()
                                              .ToListAsync();

            poster.Parking = places;

            return poster;
        }

        public async Task<List<PlaceModel>> GetPlaceList(Guid organizationId) =>
            await _context.Places.Where(p => p.ApplicationId == organizationId).ToListAsync();

        public async Task AddPoster(PosterModel model)
        {
            await _context.Posters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task AddContact(ContactModel model)
        {
            await _context.Contacts.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task AddPosterFullInfo(PosterFullInfoModel model)
        {
            await _context.PostersFullInfo.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task AddPosterMultilang(List<PosterMultilangModel> model)
        {
            await _context.PostersMultilang.AddRangeAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task AddViewLog(PosterViewLogModel model)
        {
            await _context.PosterViewLogs.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePoster(PosterModel model)
        {
            _context.Posters.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateContact(ContactModel model)
        {
            _context.Contacts.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePosterFullInfo(PosterFullInfoModel model)
        {
            _context.PostersFullInfo.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePosterMultilang(List<PosterMultilangModel> model)
        {
            _context.PostersMultilang.UpdateRange(model);
            await _context.SaveChangesAsync();
        }

        public async Task AddPlaces(List<PlaceModel> places)
        {
            await _context.Places.AddRangeAsync(places);
            await _context.SaveChangesAsync();
        }

        public async Task SaveFiles(List<FileURLModel> list, Guid posterId)
        {
            var old = await _context.FileUrls.Where(f => f.PosterId == posterId).ToListAsync();
            if (old.Count > 0)
                _context.FileUrls.RemoveRange(old);
            await _context.FileUrls.AddRangeAsync(list);
            await _context.SaveChangesAsync();
        }

        public async Task AddFilePoster(FileToApplication file)
        {
            await _context.FileToApplication.AddAsync(file);
            await _context.SaveChangesAsync();
        }

        public async Task AddSelectelFile(SelectelFileURLModel file)
        {
            await _context.SelectelFileURLs.AddAsync(file);
            await _context.SaveChangesAsync();
        }

        public async Task RemovePlaceList(List<PlaceModel> model)
        {
            _context.Places.RemoveRange(model);
            await _context.SaveChangesAsync();
        }
    }
}
