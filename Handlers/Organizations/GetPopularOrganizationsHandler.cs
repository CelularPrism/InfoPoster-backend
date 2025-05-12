using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Selectel_API;
using MediatR;

namespace InfoPoster_backend.Handlers.Organizations
{
    public class GetPopularOrganizationsRequest : IRequest<List<OrganizationResponseModel>> { }

    public class GetPopularOrganizationsHandler : IRequestHandler<GetPopularOrganizationsRequest, List<OrganizationResponseModel>>
    {
        private readonly OrganizationRepository _repository;
        private readonly SelectelAuthService _selectel;

        public GetPopularOrganizationsHandler(OrganizationRepository repository, SelectelAuthService selectel)
        {
            _repository = repository;
            _selectel = selectel;
        }

        public async Task<List<OrganizationResponseModel>> Handle(GetPopularOrganizationsRequest request, CancellationToken cancellationToken = default)
        {
            var logs = await _repository.GetPublishedViewLogs();
            var list = logs.GroupBy(l => l.PosterId).Select(l => new { Organization = l.Key, Count = l.Count() }).OrderByDescending(l => l.Count).Take(9).Select(l => l.Organization).ToList();
            var result = await _repository.GetOrganizationList(list);

            return result;
        }
    }
}
