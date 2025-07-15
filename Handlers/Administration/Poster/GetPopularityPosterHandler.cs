using InfoPoster_backend.Handlers.Administration.Organization;
using InfoPoster_backend.Models.Administration;
using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Administration.Poster
{
    public class GetPopularityPosterRequest : IRequest<List<GetPopularityPosterResponse>>
    {
        public POPULARITY_PLACE Place { get; set; }
        public Guid CityId { get; set; }
    }

    public class GetPopularityPosterResponse
    {
        public Guid? Id { get; set; }
        public Guid PosterId { get; set; }
        public string Name { get; set; }
        public int? Popularity { get; set; }
    }

    public class GetPopularityPosterHandler : IRequestHandler<GetPopularityPosterRequest, List<GetPopularityPosterResponse>>
    {
        private readonly PosterRepository _repository;
        public GetPopularityPosterHandler(PosterRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<GetPopularityPosterResponse>> Handle(GetPopularityPosterRequest request, CancellationToken cancellation = default)
        {
            var posters = await _repository.GetPopularPosterList(request.Place, request.CityId);
            var popularity = await _repository.GetPopularityList(request.Place, request.CityId);

            var result = posters.Select(o => new GetPopularityPosterResponse()
            {
                Id = popularity.Any(p => p.ApplicationId == o.Id) ? popularity.Where(p => p.ApplicationId == o.Id).Select(p => p.Id).FirstOrDefault() : null,
                PosterId = o.Id,
                Name = o.Name,
                Popularity = popularity.Any(p => p.ApplicationId == o.Id) ? popularity.Where(p => p.ApplicationId == o.Id).Select(p => p.Popularity).FirstOrDefault() : null
            }).ToList();
            return result;
        }
    }
}
