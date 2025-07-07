using InfoPoster_backend.Models.Administration;
using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Selectel_API;
using MediatR;

namespace InfoPoster_backend.Handlers.Organizations
{
    public class GetPopularOrganizationsRequest : IRequest<List<OrganizationResponseModel>>
    {
        public POPULARITY_PLACE Place { get; set; }
        public Guid? CategoryId { get; set; }
    }

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
            var result = new List<OrganizationResponseModel>();
            if (request.CategoryId != null && request.CategoryId != Guid.Empty)
            {
                result = await _repository.GetPopularOrganizationListByCategory(request.Place, (Guid)request.CategoryId);
            } else
            {
                result = await _repository.GetPopularOrganizationList(request.Place);
            }

            result = result.Where(o => o.Status == (int)POSTER_STATUS.PUBLISHED).ToList();

            return result;
        }
    }
}
