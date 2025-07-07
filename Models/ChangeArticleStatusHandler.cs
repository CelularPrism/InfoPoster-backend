using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Models
{
    public class ChangeArticleStatusRequest : IRequest<ChangeArticleStatusResponse>
    {
        public Guid Id { get; set; }
        public POSTER_STATUS Status { get; set; }
    }

    public class ChangeArticleStatusResponse { }

    public class ChangeArticleStatusHandler : IRequestHandler<ChangeArticleStatusRequest, ChangeArticleStatusResponse>
    {
        private readonly ArticleRepository _repository;

        public ChangeArticleStatusHandler(ArticleRepository repository)
        {
            _repository = repository;
        }

        public async Task<ChangeArticleStatusResponse> Handle(ChangeArticleStatusRequest request, CancellationToken cancellationToken)
        {
            var article = await _repository.GetArticle(request.Id);
            if (article == null)
                return null;

            article.Status = request.Status;
            await _repository.UpdateArticle(article);
            return new ChangeArticleStatusResponse();
        }
    }
}
