using InfoPoster_backend.Models.Organizations;
using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Administration.Organization
{
    public class GetPublishedOrganizationRequest : IRequest<List<OrganizationModel>>
    {
        public string SearchText { get; set; }
        public Guid CityId { get; set; }
    }

    public class GetPublishedOrganizationHandler : IRequestHandler<GetPublishedOrganizationRequest, List<OrganizationModel>>
    {
        private readonly OrganizationRepository _repository;

        public GetPublishedOrganizationHandler(OrganizationRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<OrganizationModel>> Handle(GetPublishedOrganizationRequest request, CancellationToken cancellation = default)
        {
            var organizations = await _repository.GetOrganizationList(request.CityId);
            //var popularity = await _repository.GetPopularityList(Models.Administration.POPULARITY_PLACE.MAIN);

            var result = organizations.Where(o => o.Name.Contains(request.SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
            return result;
        }
    }
}
