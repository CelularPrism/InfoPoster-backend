using InfoPoster_backend.Handlers.Organizations;
using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Administration;
using InfoPoster_backend.Models.Contexts;
using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Tools;
using Microsoft.EntityFrameworkCore;

namespace InfoPoster_backend.Repos
{
    public class ArticleRepository
    {
        private readonly ArticleContext _context;
        private readonly AccountContext _account;
        private readonly string _lang;
        private readonly Guid _city;

        public ArticleRepository(ArticleContext context, AccountContext account, IHttpContextAccessor accessor)
        {
            _context = context;
            _account = account;
            _lang = accessor.HttpContext.Items[Constants.HTTP_ITEM_ClientLang].ToString();
            _city = Guid.TryParse(accessor.HttpContext.Request.Headers["X-Testing"].ToString(), out _city) ? Guid.Parse(accessor.HttpContext.Request.Headers["X-Testing"].ToString()) : Constants.DefaultCity;
        }

        public async Task<ArticleModel> GetArticle(Guid id) => await _context.Articles.FirstOrDefaultAsync(x => x.Id == id);

        public async Task<List<ArticleResponse>> GetArticleList() => await _context.Articles.Join(_context.Users,
                                                                                                            a => a.UserId,
                                                                                                            u => u.Id,
                                                                                                            (a, u) => new ArticleResponse()
                                                                                                            {
                                                                                                                Id = a.Id,
                                                                                                                UserId = a.UserId,
                                                                                                                Body = a.Body,
                                                                                                                Lang = a.Lang,
                                                                                                                Title = a.Title,
                                                                                                                UserName = u.FirstName + " " + u.LastName,
                                                                                                                Status = (int)a.Status,
                                                                                                                CreatedAt = a.CreatedAt
                                                                                                            }).ToListAsync();

        public async Task<List<Guid>> GetRoles(Guid userId) => await _account.User_To_Roles.Where(u => u.UserId == userId).Select(u => u.RoleId).ToListAsync();

        public async Task<List<ArticleResponse>> GetArticleList(Guid userId)
        {
            var roles = await GetRoles(userId);
            var isAdmin = roles.Any(r => r == Constants.ROLE_ADMIN);

            var result = await _context.Articles.Where(a => isAdmin ? true : a.UserId == userId)
                                                .Join(_context.Users,
                                                      a => a.UserId,
                                                      u => u.Id,
                                                      (a, u) => new ArticleResponse()
                                                      {
                                                        Id = a.Id,
                                                        UserId = a.UserId,
                                                        Body = a.Body,
                                                        Lang = a.Lang,
                                                        Title = a.Title,
                                                        UserName = u.FirstName + " " + u.LastName,
                                                        Status = (int)a.Status,
                                                        CreatedAt = a.CreatedAt
                                                      }).ToListAsync();

            return result;
        }

        public async Task<List<ArticleResponse>> GetArticleList(POSTER_STATUS status) => await _context.Articles.Where(a => a.Status == status)
                                                                                                                .Join(_context.Users,
                                                                                                                      a => a.UserId,
                                                                                                                      u => u.Id,
                                                                                                                      (a, u) => new ArticleResponse()
                                                                                                                      {
                                                                                                                        Id = a.Id,
                                                                                                                        UserId = a.UserId,
                                                                                                                        Body = a.Body,
                                                                                                                        Lang = a.Lang,
                                                                                                                        Title = a.Title,
                                                                                                                        UserName = u.FirstName + " " + u.LastName,
                                                                                                                        Status = (int)a.Status,
                                                                                                                        CreatedAt = a.CreatedAt,
                                                                                                                        ShortDescription = a.ShortDescription
                                                                                                                      }).ToListAsync();

        public async Task<List<ArticleModel>> GetArticleListByStatus(POSTER_STATUS status) => await _context.Articles.Where(a => a.Status == status).ToListAsync();

        public async Task<List<PopularityModel>> GetPopularityList(POPULARITY_PLACE place, Guid city)
        {
            var publishedOrgs = await _context.Articles.Where(o => o.Status == POSTER_STATUS.PUBLISHED).Select(o => o.Id).ToListAsync();
            var result = await _context.Popularity.Where(p => publishedOrgs.Contains(p.ApplicationId) && 
                                                              p.Place == place &&
                                                              p.CityId == city &&
                                                              p.Type == POPULARITY_TYPE.ARTICLE).ToListAsync();
            return result;
        }

        public async Task<List<ArticleModel>> GetPopularArticleList(POPULARITY_PLACE place, Guid city)
        {
            var popularityArticles = await _context.Popularity.Where(p => p.Place == place && p.CityId == city).OrderBy(p => p.Popularity).Select(p => p.ApplicationId).ToListAsync();

            var articleList = await _context.Articles.Where(ml => popularityArticles.Contains(ml.Id)).ToListAsync();
            var result = new List<ArticleModel>();
            foreach (var item in popularityArticles)
            {
                var article = articleList.Where(o => o.Id == item).FirstOrDefault();
                if (article != null)
                    result.Add(article);
            }

            return result;
        }

        public async Task AddArticle(ArticleModel model)
        {
            await _context.Articles.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateArticle(ArticleModel model)
        {
            _context.Articles.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveArticle(ArticleModel model)
        {
            _context.Articles.Remove(model);
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
    }
}
