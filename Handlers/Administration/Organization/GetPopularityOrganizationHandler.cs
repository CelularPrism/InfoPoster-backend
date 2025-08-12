using InfoPoster_backend.Models.Administration;
using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Administration.Organization
{
    public class GetPopularityOrganizationRequest : IRequest<List<GetPopularityOrganizationResponse>>
    {
        public POPULARITY_PLACE Place { get; set; }
        public Guid CityId { get; set; }
        public Guid? PlaceId { get; set; }
    }

    public class GetPopularityOrganizationResponse
    {
        public Guid? Id { get; set; }
        public Guid OrganizationId { get; set; }
        public string Name { get; set; }
        public int? Popularity { get; set; }
    }

    public class GetPopularityOrganizationHandler : IRequestHandler<GetPopularityOrganizationRequest, List<GetPopularityOrganizationResponse>>
    {
        private readonly OrganizationRepository _repository;

        public GetPopularityOrganizationHandler(OrganizationRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<GetPopularityOrganizationResponse>> Handle(GetPopularityOrganizationRequest request, CancellationToken cancellation = default)
        {
            var organizations = await _repository.GetPopularOrganizationList(request.Place, request.CityId, request.PlaceId);
            var popularity = await _repository.GetPopularityList(request.Place, request.CityId, request.PlaceId);

            var result = organizations.Select(o => new GetPopularityOrganizationResponse()
            {
                Id = popularity.Any(p => p.ApplicationId == o.Id) ? popularity.Where(p => p.ApplicationId == o.Id).Select(p => p.Id).FirstOrDefault() : null,
                OrganizationId = o.Id,
                Name = o.Name,
                Popularity = popularity.Any(p => p.ApplicationId == o.Id) ? popularity.Where(p => p.ApplicationId == o.Id).Select(p => p.Popularity).FirstOrDefault() : null
            }).ToList();
            return result;
        }
    }
}
