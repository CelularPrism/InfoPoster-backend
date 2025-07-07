using InfoPoster_backend.Models.Organizations;
using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Administration.Organization
{
    public class GetPublishedOrganizationRequest : IRequest<List<OrganizationModel>> { }

    public class GetPublishedOrganizationHandler : IRequestHandler<GetPublishedOrganizationRequest, List<OrganizationModel>>
    {
        private readonly OrganizationRepository _repository;

        public GetPublishedOrganizationHandler(OrganizationRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<OrganizationModel>> Handle(GetPublishedOrganizationRequest request, CancellationToken cancellation = default)
        {
            var organizations = await _repository.GetOrganizationList();
            var popularity = await _repository.GetPopularityList(Models.Administration.POPULARITY_PLACE.MAIN);

            var result = organizations.Where(o => !popularity.Select(p => p.ApplicationId).Contains(o.Id)).ToList();
            return result;
        }
    }
}
