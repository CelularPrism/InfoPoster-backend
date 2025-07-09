using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Articles.Popularity
{
    public class GetPublishedArticleRequest : IRequest<List<ArticleModel>>
    {
        public string SearchText { get; set; }
    }

    public class GetPublishedArticleHandler : IRequestHandler<GetPublishedArticleRequest, List<ArticleModel>>
    {
        private readonly ArticleRepository _repository;

        public GetPublishedArticleHandler(ArticleRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ArticleModel>> Handle(GetPublishedArticleRequest request, CancellationToken cancellation = default)
        {
            var posters = await _repository.GetArticleListByStatus(POSTER_STATUS.PUBLISHED);

            var result = posters.Where(o => o.Title.Contains(request.SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
            return result;
        }
    }
}
