using InfoPoster_backend.Handlers.Administration.Poster;
using InfoPoster_backend.Models.Administration;
using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Articles.Popularity
{
    public class GetPopularityArticleRequest : IRequest<List<GetPopularityArticleResponse>>
    {
        public POPULARITY_PLACE Place { get; set; }
    }

    public class GetPopularityArticleResponse
    {
        public Guid? Id { get; set; }
        public Guid ArticleId { get; set; }
        public string Title { get; set; }
        public int? Popularity { get; set; }
    }

    public class GetPopularityArticleHandler : IRequestHandler<GetPopularityArticleRequest, List<GetPopularityArticleResponse>>
    {
        private readonly ArticleRepository _repository;
        public GetPopularityArticleHandler(ArticleRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<GetPopularityArticleResponse>> Handle(GetPopularityArticleRequest request, CancellationToken cancellation = default)
        {
            var articles = await _repository.GetPopularArticleList(request.Place);
            var popularity = await _repository.GetPopularityList(request.Place);

            var result = articles.Select(o => new GetPopularityArticleResponse()
            {
                Id = popularity.Any(p => p.ApplicationId == o.Id) ? popularity.Where(p => p.ApplicationId == o.Id).Select(p => p.Id).FirstOrDefault() : null,
                ArticleId = o.Id,
                Title = o.Title,
                Popularity = popularity.Any(p => p.ApplicationId == o.Id) ? popularity.Where(p => p.ApplicationId == o.Id).Select(p => p.Popularity).FirstOrDefault() : null
            }).ToList();
            return result;
        }
    }
}
