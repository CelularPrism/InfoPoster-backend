using InfoPoster_backend.Handlers.Administration.Organization;
using InfoPoster_backend.Models.Administration;
using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Administration.Poster
{
    public class GetPopularityPosterRequest : IRequest<List<GetPopularityPosterResponse>>
    {
        public POPULARITY_PLACE Place { get; set; }
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
            var organizations = await _repository.GetPopularOrganizationList(request.Place);
            var popularity = await _repository.GetPopularityList(request.Place);

            var result = organizations.Select(o => new GetPopularityPosterResponse()
            {
                Id = popularity.Where(p => p.ApplicationId == o.Id).Select(p => p.Id).FirstOrDefault(),
                PosterId = o.Id,
                Name = o.Name,
                Popularity = popularity.Where(p => p.ApplicationId == o.Id).Select(p => p.Popularity).FirstOrDefault()
            }).ToList();
            return result;
        }
    }
}
