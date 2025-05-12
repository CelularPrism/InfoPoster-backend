using InfoPoster_backend.Models;
using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Articles
{
    public class GetArticleRequest : IRequest<ArticleResponse>
    {
        public Guid Id { get; set; }
    }

    public class GetArticleHandler : IRequestHandler<GetArticleRequest, ArticleResponse>
    {
        private readonly ArticleRepository _article;
        private readonly AccountRepository _account;

        public GetArticleHandler(ArticleRepository article, AccountRepository account)
        {
            _article = article;
            _account = account;
        }

        public async Task<ArticleResponse> Handle(GetArticleRequest request, CancellationToken cancellationToken)
        {
            var article = await _article.GetArticle(request.Id);
            if (article == null) 
                return null;

            var user = await _account.GetUser(article.UserId);
            var result = new ArticleResponse()
            {
                UserId = user.Id,
                Title = article.Title,
                Body = article.Body,
                Id = article.Id,
                Lang = article.Lang,
                UserName = user.FirstName + " " + user.LastName,
                Status = (int)article.Status,
                CreatedAt = article.CreatedAt
            };

            return result;
        }
    }
}
