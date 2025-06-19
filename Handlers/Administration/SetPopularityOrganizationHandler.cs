using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Administration;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Login;
using MediatR;

namespace InfoPoster_backend.Handlers.Administration
{
    public class SetPopularityOrganizationRequest : IRequest<bool>
    {
        public Guid OrganizationId { get; set; }
        public Guid CategoryId { get; set; }
        public Guid SubcategoryId { get; set; }
        public int Popularity { get; set; }
    }

    public class SetPopularityOrganizationHandler : IRequestHandler<SetPopularityOrganizationRequest, bool>
    {
        private readonly OrganizationRepository _repository;
        private readonly Guid _user;

        public SetPopularityOrganizationHandler(OrganizationRepository repository, LoginService loginService)
        {
            _repository = repository;
            _user = loginService.GetUserId();
        }

        public async Task<bool> Handle(SetPopularityOrganizationRequest request, CancellationToken cancellationToken = default)
        {
            var organization = await _repository.GetOrganization(request.OrganizationId);
            if (organization == null)
                return false;

            var oldPopularity = await _repository.GetPopularity(request.OrganizationId, request.CategoryId, request.SubcategoryId);
            var oldValue = oldPopularity != null ? string.Empty : oldPopularity.Popularity.ToString();

            var popularity = oldPopularity != null ? oldPopularity : new PopularityModel();
            popularity.ApplicationId = request.OrganizationId;
            popularity.CategoryId = request.CategoryId;
            popularity.SubcategoryId = request.SubcategoryId;
            popularity.Popularity = request.Popularity;

            var changeHistory = new List<ApplicationChangeHistory>() {
                new ApplicationChangeHistory(Guid.NewGuid(), organization.Id, "Popularity", oldValue, request.Popularity.ToString(), _user)
            };

            await _repository.AddHistory(changeHistory);
            if (oldPopularity != null)
                await _repository.UpdatePopularity(popularity);
            else
                await _repository.AddPopularity(popularity);

            return true;
        }
    }
}
