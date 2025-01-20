using InfoPoster_backend.Models;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Login;
using MediatR;

namespace InfoPoster_backend.Handlers.Articles
{
    public class SaveArticleRequest : IRequest<SaveArticleResponse>
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Lang { get; set; }
    }

    public class SaveArticleResponse
    {
        public Guid Id { get; set; }
    }

    public class SaveArticleHandler : IRequestHandler<SaveArticleRequest, SaveArticleResponse> 
    {
        private readonly ArticleRepository _repository;

        public SaveArticleHandler(ArticleRepository repository)
        {
            _repository = repository;
        }

        public async Task<SaveArticleResponse> Handle(SaveArticleRequest request, CancellationToken cancellationToken = default)
        {
            var article = await _repository.GetArticle(request.Id);

            if (article == null)
                return null;

            article.Title = request.Title;
            article.Body = request.Body;
            article.Lang = request.Lang;
            await _repository.UpdateArticle(article);

            return new SaveArticleResponse() { Id = article.Id };
        }
    }
}
