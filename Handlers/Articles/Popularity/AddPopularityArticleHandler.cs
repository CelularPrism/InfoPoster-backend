using InfoPoster_backend.Handlers.Administration.Organization;
using InfoPoster_backend.Models.Administration;
using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Articles.Popularity
{
    public class AddPopularityArticleRequest : IRequest<AddPopularityArticleResponse>
    {
        public POPULARITY_PLACE Place { get; set; }
        public List<PopularityRequestModel> Popularity { get; set; }
        public Guid CityId { get; set; }
    }

    public class AddPopularityArticleResponse
    {

    }

    public class AddPopularityArticleHandler : IRequestHandler<AddPopularityArticleRequest, AddPopularityArticleResponse>
    {
        private readonly ArticleRepository _repository;

        public AddPopularityArticleHandler(ArticleRepository repository)
        {
            _repository = repository;
        }

        public async Task<AddPopularityArticleResponse> Handle(AddPopularityArticleRequest request, CancellationToken cancellation = default)
        {
            var anyIdentical = request.Popularity.GroupBy(p => p.Id).Select(p => new { Id = p.Key, Count = p.Count() }).Any(p => p.Count > 1);
            if (anyIdentical)
            {
                return null;
            }

            var popularity = await _repository.GetPopularityList(request.Place, request.CityId);
            var addList = new List<PopularityModel>();

            foreach (var item in request.Popularity)
            {
                addList.Add(new PopularityModel()
                {
                    Id = Guid.NewGuid(),
                    ApplicationId = item.Id,
                    Place = request.Place,
                    Popularity = item.Popularity,
                    Type = POPULARITY_TYPE.ARTICLE,
                    CityId = request.CityId
                });
            }

            await _repository.RemovePopularity(popularity);
            await _repository.AddPopularity(addList);
            return new AddPopularityArticleResponse();
        }
    }
}
