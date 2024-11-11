using InfoPoster_backend.Models.Cities;
using InfoPoster_backend.Models.Organizations;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Login;
using MediatR;

namespace InfoPoster_backend.Handlers.Administration
{
    public class SearchOrganizationRequest : IRequest<List<OrganizationModel>>
    {
        public string SearchText { get; set; }
        public Guid CityId { get; set; }
    }

    public class SearchOrganizationHandler : IRequestHandler<SearchOrganizationRequest, List<OrganizationModel>>
    {
        private readonly OrganizationRepository _repository;

        public SearchOrganizationHandler(OrganizationRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<OrganizationModel>> Handle(SearchOrganizationRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _repository.SearchOrganizationList(request.SearchText, request.CityId);
            return result;
        }
    }
}
