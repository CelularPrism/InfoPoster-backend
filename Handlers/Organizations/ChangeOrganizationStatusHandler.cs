using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
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
        public ChangeOrganizationStatusHandler(OrganizationRepository repository)
        {
            _repository = repository;
        }

        public async Task<ChangeOrganizationStatusResponse> Handle(ChangeOrganizationStatusRequest request, CancellationToken cancellationToken = default)
        {
            var organization = await _repository.GetOrganization(request.Id);
            if (organization == null)
                return null;

            organization.Status = (int)request.Status;
            await _repository.UpdateOrganization(organization);
            return new ChangeOrganizationStatusResponse();
        }
    }
}
