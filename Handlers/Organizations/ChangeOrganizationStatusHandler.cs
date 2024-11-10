using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Login;
using MediatR;

namespace InfoPoster_backend.Handlers.Organizations
{
    public class ChangeOrganizationStatusRequest : IRequest<ChangeOrganizationStatusResponse>
    {
        public Guid Id { get; set; }
        public POSTER_STATUS Status { get; set; }
    }

    public class ChangeOrganizationStatusResponse { }

    public class ChangeOrganizationStatusHandler : IRequestHandler<ChangeOrganizationStatusRequest, ChangeOrganizationStatusResponse>
    {
        private readonly OrganizationRepository _repository;
        private readonly Guid _user;

        public ChangeOrganizationStatusHandler(OrganizationRepository repository, LoginService loginService)
        {
            _repository = repository;
            _user = loginService.GetUserId();
        }

        public async Task<ChangeOrganizationStatusResponse> Handle(ChangeOrganizationStatusRequest request, CancellationToken cancellationToken = default)
        {
            var organization = await _repository.GetOrganization(request.Id);
            if (organization == null)
                return null;

            organization.Status = (int)request.Status;
            await _repository.UpdateOrganization(organization, _user);
            return new ChangeOrganizationStatusResponse();
        }
    }
}
