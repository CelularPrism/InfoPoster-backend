using InfoPoster_backend.Models;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Login;
using MediatR;

namespace InfoPoster_backend.Handlers.Articles
{
    public class CreateArticleRequest : IRequest<CreateArticleResponse> { }

    public class CreateArticleResponse
    {
        public Guid Id { get; set; }
    }

    public class CreateArticleHandler : IRequestHandler<CreateArticleRequest, CreateArticleResponse>
    {
        private readonly ArticleRepository _repository;
        private readonly Guid _user;

        public CreateArticleHandler(ArticleRepository repository, LoginService loginService)
        {
            _repository = repository;
            _user = loginService.GetUserId();
        }

        public async Task<CreateArticleResponse> Handle(CreateArticleRequest request, CancellationToken cancellationToken = default)
        {
            var article = new ArticleModel()
            {
                UserId = _user,
                Status = Models.Posters.POSTER_STATUS.DRAFT
            };

            await _repository.AddArticle(article);
            return new CreateArticleResponse() { Id = article.Id };
        }
    }
}
