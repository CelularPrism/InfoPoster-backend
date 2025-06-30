using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Contexts;
using Microsoft.EntityFrameworkCore;

namespace InfoPoster_backend.Repos
{
    public class ArticleRepository
    {
        private readonly ArticleContext _context;

        public ArticleRepository(ArticleContext context)
        {
            _context = context;
        }

        public async Task<ArticleModel> GetArticle(Guid id) => await _context.Articles.FirstOrDefaultAsync(x => x.Id == id);

        public async Task<List<ArticleResponse>> GetArticleList(Guid userId) => await _context.Articles.Where(a => a.UserId == userId)
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
    }
}
