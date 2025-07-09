using InfoPoster_backend.Handlers.Administration;
using InfoPoster_backend.Handlers.Organizations;
using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Account;
using InfoPoster_backend.Models.Administration;
using InfoPoster_backend.Models.Cities;
using InfoPoster_backend.Models.Contexts;
using InfoPoster_backend.Models.Organizations;
using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Models.Selectel;
using InfoPoster_backend.Tools;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
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

        public async Task<List<PosterModel>> GetPosterList() =>
            await _context.Posters.Where(p => p.Status == (int)POSTER_STATUS.PUBLISHED)
                                  .Join(_context.PostersMultilang,
                                        p => p.Id,
                                        ml => ml.PosterId,
                                        (p, ml) => new { p, ml })
                                  .Where(p => p.ml.Lang == _lang)
                                  .Select(p => new PosterModel()
                                  {
                                      Id = p.p.Id,
                                      CategoryId = p.p.CategoryId,
                                      CreatedAt = p.p.CreatedAt,
                                      Name = p.ml.Name,
                                      ReleaseDate = p.p.ReleaseDate,
                                      ReleaseDateEnd = p.p.ReleaseDateEnd,
                                      Status = p.p.Status,
                                      SubcategoryId = p.p.SubcategoryId,
                                      UpdatedAt = p.p.UpdatedAt,
                                      UserId = p.p.UserId
                                  }).ToListAsync();

        public async Task<List<PosterModel>> GetPosterListByUserId(Guid userId) =>
            await _context.Posters.Where(p => p.UserId == userId).ToListAsync();

        public async Task<int> GetCountByStatus(int status) =>
            await _context.Posters.Where(p => p.Status == status).CountAsync();

        public async Task<List<CategoryModel>> GetCategories() =>
            await _context.CategoriesMultilang.Where(c => c.lang == _lang)
                                              .Join(_context.Categories,
                                                    c => c.CategoryId,
                                                    category => category.Id,
                                                    (c, cat) => new CategoryModel()
                                                    {
                                                        Id = cat.Id,
                                                        ImageSrc = cat.ImageSrc,
                                                        Name = c.Name,
                                                        Type = cat.Type
                                                    }).ToListAsync();

        public async Task<List<SubcategoryModel>> GetSubcategories() => await _context.Subcategories.Join(_context.SubcategoriesMultilang,
                                                                                                      c => c.Id,
                                                                                                      m => m.SubcategoryId,
                                                                                                      (c, m) => new { Subcategory = c, Multilang = m })
                                                                                                .Where(c => c.Multilang.lang == _lang)
                                                                                                .Select(c => new SubcategoryModel()
                                                                                                {
                                                                                                    Id = c.Subcategory.Id,
                                                                                                    Name = c.Multilang.Name,
                                                                                                    ImageSrc = c.Subcategory.ImageSrc
                                                                                                }).ToListAsync();

        public async Task<bool> CheckAdmin(Guid userId) =>
            await _context.User_To_Roles.AnyAsync(us => us.UserId == userId && us.RoleId == Constants.ROLE_ADMIN);

        public async Task<PosterFullInfoModel> GetFullInfoPoster(Guid posterId) =>
            await _context.PostersFullInfo.FirstOrDefaultAsync(x => x.PosterId == posterId);

        public async Task<PosterMultilangModel> GetMultilangPoster(Guid posterId, string lang) =>
            await _context.PostersMultilang.FirstOrDefaultAsync(x => x.PosterId == posterId && x.Lang == lang);

        public async Task<List<PosterMultilangModel>> GetMultilangPosterList(Guid posterId) =>
            await _context.PostersMultilang.Where(x => x.PosterId == posterId).ToListAsync();

        public async Task<ContactModel> GetContact(Guid posterId) =>
            await _context.Contacts.FirstOrDefaultAsync(c => c.ApplicationId == posterId && c.Lang == _lang);

        public async Task<List<FileURLModel>> GetFileUrls(Guid posterId) =>
            await _context.FileUrls.Where(f => f.PosterId == posterId).ToListAsync();

        public async Task<List<PlaceModel>> GetPlaces(Guid organizationId) =>
            await _context.Places.Where(p => p.ApplicationId == organizationId && p.Lang == _lang).ToListAsync();

        public async Task<List<PosterModel>> SearchPosters(string searchText) => await _context.PostersMultilang.Where(p => p.Name.Contains(searchText) && p.Lang == _lang)
                                                                                                                .Join(_context.Posters,
                                                                                                                      ml => ml.PosterId,
                                                                                                                      p => p.Id,
                                                                                                                      (ml, p) => new PosterModel()
                                                                                                                      {
                                                                                                                          Id = ml.PosterId,
                                                                                                                          CategoryId = p.CategoryId,
                                                                                                                          CreatedAt = p.CreatedAt,
                                                                                                                          Name = p.Name,
                                                                                                                          ReleaseDate = p.ReleaseDate,
                                                                                                                          ReleaseDateEnd = p.ReleaseDateEnd,
                                                                                                                          Status = p.Status,
                                                                                                                          UpdatedAt = p.UpdatedAt,
                                                                                                                          UserId = p.UserId
                                                                                                                      })
                                                                                                                .OrderByDescending(p => p.ReleaseDate)
                                                                                                                .Take(5)
                                                                                                                .AsNoTracking().ToListAsync();

        public async Task<SelectelFileURLModel> GetSelectelFile(Guid organizationId) =>
            await _context.FileToApplication.Where(f => f.ApplicationId == organizationId)
                                                 .Join(_context.SelectelFileURLs,
                                                       f => f.FileId,
                                                       s => s.Id,
                                                       (f, s) => s)
                                                 .FirstOrDefaultAsync();

        public async Task<List<PosterModel>> GetListNoTracking(string lang, Guid adminId, List<int> statuses, Guid? categoryId, Guid? subcategoryId, DateTime? startDate, DateTime? endDate, Guid? userId, Guid? cityId)
        {
            var isAdmin = await _context.User_To_Roles.AnyAsync(u => u.UserId == adminId && u.RoleId == Constants.ROLE_ADMIN);
            //var query = _context.Posters.Where(p => p.Status == (int)POSTER_STATUS.PENDING ||
            //                                        p.Status == (int)POSTER_STATUS.PUBLISHED ||
            //                                        p.Status == (int)POSTER_STATUS.DRAFT ||
            //                                        p.Status == (isAdmin ? (int)POSTER_STATUS.REVIEWING : (int)POSTER_STATUS.DELETED));
            var query = _context.Posters.Where(p => statuses.Contains(p.Status));

            query = FilterPosters(query, categoryId, subcategoryId, null, startDate, endDate, userId, cityId);
            var result = await query.ToListAsync();
            return result;
        }

        public async Task<List<PosterModel>> GetRejectedListNoTracking(string lang, Guid adminId, Guid? categoryId, Guid? subcategoryId, int? status, DateTime? startDate, DateTime? endDate, Guid? userId, Guid? cityId)
        {
            var query = _context.Posters.Where(p => p.Status == (int)POSTER_STATUS.REJECTED);

            query = FilterPosters(query, categoryId, subcategoryId, status, startDate, endDate, userId, cityId);
            var result = await query.ToListAsync();
            return result;
        }

        public async Task<List<PosterResponseModel>> GetPosters(IEnumerable<Guid> posters)
        {
            var list = await _context.Posters.Where(p => posters.Contains(p.Id))
                                  .Join(_context.PostersFullInfo,
                                        p => p.Id,
                                        f => f.PosterId,
                                        (p, f) => new { p, f })
                                  .Join(_context.PostersMultilang,
                                        p => p.p.Id,
                                        m => m.PosterId,
                                        (p, m) => new { p, m })
                                  .Where(p => p.m.Lang == _lang)
                                  .OrderBy(p => p.p.p.ReleaseDate)
                                  .AsNoTracking()
                                  .ToListAsync();
            var categories = await _context.CategoriesMultilang.Where(c => c.lang == _lang).ToListAsync();

            var result = list.Select(p => new PosterResponseModel(p.p.p, p.m)
            {
                CategoryName = categories.Where(c => c.CategoryId == p.p.p.CategoryId).Select(c => c.Name).FirstOrDefault(),
                SubcategoryName = categories.Where(c => c.CategoryId == p.p.p.SubcategoryId).Select(c => c.Name).FirstOrDefault(),
                FileId = _context.FileToApplication.Where(f => f.ApplicationId == p.p.p.Id && f.IsPrimary).Any() ?
                                                 _context.FileToApplication.Where(f => f.ApplicationId == p.p.p.Id && f.IsPrimary).Select(f => f.FileId).FirstOrDefault() :
                                                 _context.FileToApplication.Where(f => f.ApplicationId == p.p.p.Id).Select(f => f.FileId).FirstOrDefault(),
                Price = p.p.f.Price,
            })
                             .OrderBy(p => p.ReleaseDate)
                             .ToList();

            return result;
        }

        public async Task<List<PosterMultilangModel>> GetMultilang(IEnumerable<Guid> posters) =>
            await _context.PostersMultilang.Where(m => m.Lang == _lang && posters.Contains(m.PosterId)).ToListAsync();

        public async Task<List<PosterFullInfoModel>> GetFullInfo(IEnumerable<Guid> posters) =>
            await _context.PostersFullInfo.Where(f => posters.Contains(f.PosterId)).ToListAsync();

        public async Task<List<UserModel>> GetUsers(IEnumerable<Guid> users) =>
            await _context.Users.Where(u => users.Contains(u.Id)).ToListAsync();

        public async Task<List<CityModel>> GetCities() =>
            await _context.CitiesMultilang.Where(c => c.Lang == _lang).Select(c => new CityModel() { Id = c.CityId, Name = c.Name }).ToListAsync();

        public async Task<List<AdministrationPostersResponse>> GetListNoTracking(Guid userId, string lang, Guid? categoryId, Guid? subcategoryId, int? status, DateTime? startDate, DateTime? endDate)
        {
            var isAdmin = await _context.User_To_Roles.AnyAsync(u => u.UserId == userId && u.RoleId == Constants.ROLE_ADMIN);

            if (!isAdmin)
            {
                var query = _context.Posters.Where(p => p.UserId == userId);
                if (categoryId != null)
                {
                    query = query.Where(p => p.CategoryId == categoryId);
                }

                if (subcategoryId != null)
                {
                    query = query.Where(p => p.SubcategoryId == subcategoryId);
                }

                if (status != null)
                {
                    query = query.Where(p => p.Status == status);
                }

                if (startDate != null)
                {
                    query = query.Where(p => p.CreatedAt >= startDate);
                }

                if (endDate != null)
                {
                    query = query.Where(p => p.CreatedAt <= endDate);
                }

                var list = await query.Join(_context.PostersMultilang,
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

                var result = list.Select(p => new AdministrationPostersResponse(p.p.p, p.p.m, p.UserName)
                {
                    CategoryName = _context.CategoriesMultilang.Where(c => c.CategoryId == p.p.p.CategoryId && c.lang == "en").Select(c => c.Name).FirstOrDefault(),
                    SubcategoryName = _context.SubcategoriesMultilang.Where(s => s.SubcategoryId == p.p.p.SubcategoryId && s.lang == "en").Select(s => s.Name).FirstOrDefault(),
                    CityName = _context.PostersFullInfo.Where(f => f.PosterId == p.p.p.Id).Select(f => f.City).Join(_context.Cities, f => f, c => c.Id, (f, c) => c.Name).FirstOrDefault(),
                    CityId = _context.PostersFullInfo.Where(f => f.PosterId == p.p.p.Id).Select(f => f.City).FirstOrDefault()
                }).OrderBy(p => p.ReleaseDate)
                  .ToList();

                return result;
            } else
            {
                var query = _context.Posters.AsQueryable();

                if (categoryId != null)
                {
                    query = query.Where(p => p.CategoryId == categoryId);
                }

                if (subcategoryId != null)
                {
                    query = query.Where(p => p.SubcategoryId == subcategoryId);
                }

                if (status != null)
                {
                    query = query.Where(p => p.Status == status);
                }

                if (startDate != null)
                {
                    query = query.Where(p => p.CreatedAt >= startDate);
                }

                if (endDate != null)
                {
                    query = query.Where(p => p.CreatedAt <= endDate);
                }

                var list = await query.Join(_context.PostersMultilang,
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

                var result = list.Select(p => new AdministrationPostersResponse(p.p.p, p.p.m, p.UserName)
                {
                    CategoryName = _context.CategoriesMultilang.Where(c => c.CategoryId == p.p.p.CategoryId && c.lang == "en").Select(c => c.Name).FirstOrDefault(),
                    SubcategoryName = _context.SubcategoriesMultilang.Where(s => s.SubcategoryId == p.p.p.SubcategoryId && s.lang == "en").Select(s => s.Name).FirstOrDefault(),
                    CityName = _context.PostersFullInfo.Where(f => f.PosterId == p.p.p.Id).Select(f => f.City).Join(_context.Cities, f => f, c => c.Id, (f, c) => c.Name).FirstOrDefault(),
                    CityId = _context.PostersFullInfo.Where(f => f.PosterId == p.p.p.Id).Select(f => f.City).FirstOrDefault()
                }).OrderBy(p => p.ReleaseDate)
                  .ToList();

                return result;
            }
        }

        public async Task<List<PosterResponseModel>> GetListNoTracking(DateTime start, DateTime end, int? count, string lang = "en")
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
                                  .OrderBy(p => p.p.p.ReleaseDate)
                                  .AsNoTracking()
                                  .ToListAsync();
            var categories = await _context.CategoriesMultilang.Where(c => c.lang == lang).ToListAsync();

            if (count != null)
            {
                list = list.Take((int)count).ToList();
            }

            var result = list.Select(p => new PosterResponseModel(p.p.p, p.m)
            {
                CategoryName = categories.Where(c => c.CategoryId == p.p.p.CategoryId).Select(c => c.Name).FirstOrDefault(),
                SubcategoryName = categories.Where(c => c.CategoryId == p.p.p.SubcategoryId).Select(c => c.Name).FirstOrDefault(),
                FileId = _context.FileToApplication.Where(f => f.ApplicationId == p.p.p.Id && f.IsPrimary).Any() ?
                                                 _context.FileToApplication.Where(f => f.ApplicationId == p.p.p.Id && f.IsPrimary).Select(f => f.FileId).FirstOrDefault() :
                                                 _context.FileToApplication.Where(f => f.ApplicationId == p.p.p.Id).Select(f => f.FileId).FirstOrDefault(),
                Price = p.p.f.Price,
            })
                             .OrderBy(p => p.ReleaseDate)
                             .ToList();
            return result;
        }

        public async Task<(List<PosterResponseModel>, int)> GetListNoTracking(int limit, int offset, DateTime start, DateTime end, Guid categoryId, string lang = "en")
        {
            var query = _context.Posters.Where(p => (p.CategoryId == categoryId || p.SubcategoryId == categoryId) && p.Status == (int)POSTER_STATUS.PUBLISHED && (p.ReleaseDate >= start.Date || p.ReleaseDateEnd <= start.Date));
            var total = query.Count();

            query = query.Skip(offset).Take(limit);

            var list = await query.Join(_context.PostersFullInfo,
                                        p => p.Id,
                                        f => f.PosterId,
                                        (p, f) => new { p, f })
                                  .Where(p => p.f.City == _city)
                                  .Join(_context.PostersMultilang,
                                        p => p.p.Id,
                                        m => m.PosterId,
                                        (p, m) => new { p, m })
                                  .Where(p => p.m.Lang == lang)
                                  .AsNoTracking()
                                  .ToListAsync();

            var categories = await _context.CategoriesMultilang.Where(c => c.lang == lang).ToListAsync();
            var subcategories = await _context.SubcategoriesMultilang.Where(s => s.lang == lang).ToListAsync();

            var result = list.Select(p => new PosterResponseModel(p.p.p, p.m)
            {
                CategoryName = categories.Where(c => c.CategoryId == p.p.p.CategoryId).Select(c => c.Name).FirstOrDefault(),
                SubcategoryName = subcategories.Where(s => s.SubcategoryId == p.p.p.SubcategoryId).Select(s => s.Name).FirstOrDefault(),
                FileId = _context.FileToApplication.Where(f => f.ApplicationId == p.p.p.Id && f.IsPrimary).Any() ?
                                                         _context.FileToApplication.Where(f => f.ApplicationId == p.p.p.Id && f.IsPrimary).Select(f => f.FileId).FirstOrDefault() :
                                                         _context.FileToApplication.Where(f => f.ApplicationId == p.p.p.Id).Select(f => f.FileId).FirstOrDefault(),
                Price = p.p.f.Price,
            })
            .OrderBy(p => p.ReleaseDate)
            .ToList();
            return (result, total);
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

            poster.SocialLinks = files.Where(f => f.FileCategory == (int)FILE_CATEGORIES.SOCIAL_LINKS).Select(f => f.URL).FirstOrDefault();
            poster.VideoUrls = files.Where(f => f.FileCategory == (int)FILE_CATEGORIES.VIDEO).Select(f => f.URL).ToList();

            var places = await _context.Places.Where(p => p.ApplicationId == Id && p.Lang == lang)
                                              .AsNoTracking()
                                              .ToListAsync();

            poster.Parking = places;
            poster.AttachedOrganizationName = await _context.OrganizationsMultilang.Where(m => m.OrganizationId == model.FullInfo.OrganizationId && m.Lang == _lang).Select(m => m.Name).FirstOrDefaultAsync();
            poster.Contacts = await _context.Contacts.Where(c => c.ApplicationId == Id && c.Lang == _lang).Select(c => c.Contacts).FirstOrDefaultAsync();

            return poster;
        }

        public async Task<List<PlaceModel>> GetPlaceList(Guid posterId) =>
            await _context.Places.Where(p => p.ApplicationId == posterId).ToListAsync();

        public async Task<List<ApplicationHistoryResponse>> GetHistoryList(Guid posterId) =>
            await _context.ApplicationHistory.Where(h => h.ApplicationId == posterId)
                                             .Join(_context.Users,
                                                   h => h.UserId,
                                                   u => u.Id,
                                                   (h, u) => new ApplicationHistoryResponse()
                                                   {
                                                       ApplicationId = h.ApplicationId,
                                                       Id = h.Id,
                                                       UpdatedAt = h.UpdatedAt,
                                                       UserId = h.UserId,
                                                       UserName = u.FirstName + " " + u.LastName,
                                                       AnyFieldsLog = _context.ApplicationChangeHistory.Any(c => c.ArticleId == h.Id)
                                                   })
                                             .OrderByDescending(h => h.UpdatedAt).ToListAsync();

        public async Task<List<ApplicationChangeHistory>> GetChangeHistory(Guid articleId) =>
            await _context.ApplicationChangeHistory.Where(h => h.ArticleId == articleId).OrderByDescending(h => h.ChangedAt).ToListAsync();

        public async Task<bool> AnyOrganization(Guid organizationId) => await _context.Organizations.AnyAsync(o => o.Id == organizationId);

        public async Task<RejectedComments> GetLastRejectedComment(Guid posterId) =>
            await _context.RejectedComments.Where(r => r.ApplicationId == posterId).OrderByDescending(r => r.CreatedAt).FirstOrDefaultAsync();

        public async Task<List<PosterViewLogModel>> GetPublishedViewLogs(DateTime startDate, DateTime endDate)
        {
            var posters = _context.Posters.Where(p => p.Status == (int)POSTER_STATUS.PUBLISHED &&
                                                      (p.ReleaseDate >= startDate || p.ReleaseDateEnd >= startDate) &&
                                                      (p.ReleaseDate <= endDate || p.ReleaseDateEnd <= endDate))
                                          .Join(_context.PostersFullInfo,
                                                p => p.Id,
                                                f => f.PosterId,
                                                (p, f) => f)
                                          .Where(p => p.City == _city)
                                          .Select(p => p.PosterId).AsEnumerable();

            var result = await _context.PosterViewLogs.Where(l => posters.Contains(l.PosterId)).ToListAsync();
            return result;
        }

        public async Task<List<PosterViewLogModel>> GetPublishedViewLogsBySubcategory(DateTime startDate, DateTime endDate, Guid subcategoryId)
        {
            var posters = _context.Posters.Where(p => p.Status == (int)POSTER_STATUS.PUBLISHED &&
                                                      p.SubcategoryId == subcategoryId &&
                                                      (p.ReleaseDate >= startDate || p.ReleaseDateEnd >= startDate) &&
                                                      (p.ReleaseDate <= endDate || p.ReleaseDateEnd <= endDate))
                                          .Join(_context.PostersFullInfo,
                                                p => p.Id,
                                                f => f.PosterId,
                                                (p, f) => f)
                                          .Where(p => p.City == _city)
                                          .Select(p => p.PosterId).AsEnumerable();

            var result = await _context.PosterViewLogs.Where(l => posters.Contains(l.PosterId)).ToListAsync();
            return result;
        }

        public async Task<List<ApplicationChangeHistory>> GetStatusHistory(DateTime startDate, string status)
        {
            var history = await _context.ApplicationChangeHistory.Where(h => h.FieldName == "Status" && h.NewValue == status && h.ChangedAt >= startDate).OrderByDescending(h => h.ChangedAt).ToListAsync();
            return history;
        }

        public async Task<List<PopularityModel>> GetPopularityList(POPULARITY_PLACE place)
        {
            var publishedOrgs = await _context.Posters.Where(o => o.Status == (int)POSTER_STATUS.PUBLISHED).Select(o => o.Id).ToListAsync();
            var result = await _context.Popularity.Where(p => publishedOrgs.Contains(p.ApplicationId) && p.Place == place).ToListAsync();
            return result;
        }

        public async Task<List<PosterResponseModel>> GetPopularOrganizationList(POPULARITY_PLACE place)
        {
            var popularityOrgs = await _context.Popularity.Where(p => p.Place == place).OrderBy(p => p.Popularity).Select(p => p.ApplicationId).ToListAsync();
            var categories = await _context.CategoriesMultilang.Where(c => c.lang == _lang).ToListAsync();
            var subcategories = await _context.SubcategoriesMultilang.Where(c => c.lang == _lang).ToListAsync();

            var posters = await _context.PostersMultilang.Where(ml => ml.Lang == _lang && popularityOrgs.Contains(ml.PosterId))
                                                                   .Join(_context.Posters,
                                                                   ml => ml.PosterId,
                                                                   p => p.Id,
                                                                   (ml, p) => new PosterResponseModel()
                                                                   {
                                                                       Id = ml.PosterId,
                                                                       CategoryId = p.CategoryId,
                                                                       CategoryName = categories.Where(c => c.Id == p.CategoryId).Select(c => c.Name).FirstOrDefault(),
                                                                       Name = ml.Name,
                                                                       SubcategoryName = subcategories.Where(s => s.Id == p.SubcategoryId).Select(s => s.Name).FirstOrDefault()
                                                                   }).ToListAsync();
            var result = new List<PosterResponseModel>();
            foreach (var item in popularityOrgs)
            {
                var poster = posters.Where(o => o.Id == item).FirstOrDefault();
                result.Add(poster);
            }

            return result;
        }

        public async Task<List<PosterResponseModel>> GetPopularOrganizationListByCategory(POPULARITY_PLACE place, Guid categoryId)
        {
            var popularityOrgs = await _context.Popularity.Where(p => p.Place == place && p.CategoryId == categoryId).OrderBy(p => p.Popularity).Select(p => p.ApplicationId).ToListAsync();
            var categories = await _context.CategoriesMultilang.Where(c => c.lang == _lang).ToListAsync();
            var subcategories = await _context.SubcategoriesMultilang.Where(c => c.lang == _lang).ToListAsync();

            var posters = await _context.PostersMultilang.Where(ml => ml.Lang == _lang && popularityOrgs.Contains(ml.PosterId))
                                                                   .Join(_context.Posters,
                                                                   ml => ml.PosterId,
                                                                   org => org.Id,
                                                                   (ml, o) => new PosterResponseModel()
                                                                   {
                                                                       Id = ml.PosterId,
                                                                       CategoryId = o.CategoryId,
                                                                       CategoryName = categories.Where(c => c.Id == o.CategoryId).Select(c => c.Name).FirstOrDefault(),
                                                                       Name = ml.Name,
                                                                       SubcategoryName = subcategories.Where(s => s.Id == o.SubcategoryId).Select(s => s.Name).FirstOrDefault()
                                                                   }).ToListAsync();
            var result = new List<PosterResponseModel>();
            foreach (var item in popularityOrgs)
            {
                var poster = posters.Where(o => o.Id == item).FirstOrDefault();
                result.Add(poster);
            }

            return result;
        }

        public async Task AddPoster(PosterModel model, Guid userId)
        {
            var history = new ApplicationHistoryModel()
            {
                ApplicationId = model.Id,
                UserId = userId
            };

            await _context.ApplicationHistory.AddAsync(history);
            await _context.Posters.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task AddPoster(List<PosterModel> model, Guid userId)
        {
            var history = model.Select(m => new ApplicationHistoryModel()
            {
                ApplicationId = m.Id,
                UserId = userId
            }).ToList();

            await _context.ApplicationHistory.AddRangeAsync(history);
            await _context.Posters.AddRangeAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task AddContact(List<ContactModel> model, Guid posterId)
        {
            var oldContacts = await _context.Contacts.Where(c => c.ApplicationId == posterId).ToListAsync();
            if (oldContacts.Count > 0)
            {
                _context.Contacts.RemoveRange(oldContacts);
            }

            await _context.Contacts.AddRangeAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task AddPosterFullInfo(PosterFullInfoModel model)
        {
            await _context.PostersFullInfo.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task AddPosterFullInfo(List<PosterFullInfoModel> model)
        {
            await _context.PostersFullInfo.AddRangeAsync(model);
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

        public async Task AddRejectedComment(RejectedComments model)
        {
            await _context.RejectedComments.AddAsync(model);
            await _context.SaveChangesAsync();
        }
 
        public async Task UpdatePoster(PosterModel model, Guid userId, Guid articleId)
        {
            var history = new ApplicationHistoryModel()
            {
                Id = articleId,
                ApplicationId = model.Id,
                UserId = userId,
            };

            _context.Posters.Update(model);
            await _context.ApplicationHistory.AddAsync(history);
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

        public async Task AddChangeHistory(List<ApplicationChangeHistory> list)
        {
            await _context.ApplicationChangeHistory.AddRangeAsync(list);
            await _context.SaveChangesAsync();
        }

        public async Task AddPopularity(PopularityModel popularity)
        {
            await _context.Popularity.AddAsync(popularity);
            await _context.SaveChangesAsync();
        }

        public async Task AddPopularity(List<PopularityModel> popularity)
        {
            await _context.Popularity.AddRangeAsync(popularity);
            await _context.SaveChangesAsync();
        }

        public async Task RemovePopularity(PopularityModel popularity)
        {
            _context.Popularity.Remove(popularity);
            await _context.SaveChangesAsync();
        }

        public async Task RemovePopularity(List<PopularityModel> popularity)
        {
            _context.Popularity.RemoveRange(popularity);
            await _context.SaveChangesAsync();
        }

        private IQueryable<PosterModel> FilterPosters(IQueryable<PosterModel> query, Guid? categoryId, Guid? subcategoryId, int? status, DateTime? startDate, DateTime? endDate, Guid? userId, Guid? cityId)
        {

            if (categoryId != null)
            {
                query = query.Where(q => q.CategoryId == categoryId);
            }

            if (subcategoryId != null)
            {
                query = query.Where(q => q.SubcategoryId == subcategoryId);
            }

            if (status != null)
            {
                query = query.Where(q => q.Status == status);
            }

            if (startDate != null)
            {
                query = query.Where(q => q.CreatedAt >= startDate);
            }

            if (endDate != null)
            {
                query = query.Where(q => q.CreatedAt <= endDate);
            }

            if (userId != null)
            {
                query = query.Where(q => q.UserId == userId);
            }

            if (cityId != null)
            {
                query = query.Join(_context.PostersFullInfo,
                                   q => q.Id,
                                   f => f.PosterId,
                                   (q, f) => new { Poster = q, CityId = f.City })
                             .Where(q => q.CityId == cityId)
                             .Select(q => q.Poster);
            }
            return query;
        }
    }
}
