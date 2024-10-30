using InfoPoster_backend.Models.Organizations;
using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Login;
using InfoPoster_backend.Tools;
using MediatR;

namespace InfoPoster_backend.Handlers.Organizations
{
    public class CreateOrganizationRequest : IRequest<CreateOrganizationResponse> { }

    public class CreateOrganizationResponse
    {
        public Guid Id { get; set; }
    }

    public class CreateOrganizationHandler : IRequestHandler<CreateOrganizationRequest, CreateOrganizationResponse>
    {
        private readonly OrganizationRepository _repository;
        private readonly LoginService _loginService;
        public CreateOrganizationHandler(OrganizationRepository repository, LoginService loginService)
        {
            _repository = repository;
            _loginService = loginService;
        }

        public async Task<CreateOrganizationResponse> Handle(CreateOrganizationRequest request, CancellationToken cancellationToken = default)
        {
            var userId = _loginService.GetUserId();
            var organization = new OrganizationModel()
            {
                UserId = userId,
                Status = (int)POSTER_STATUS.DRAFT
            };

            var multilang = new List<OrganizationMultilangModel>(); ;
            foreach (var lang in Constants.SystemLangs)
            {
                multilang.Add(new OrganizationMultilangModel()
                {
                    Id = Guid.NewGuid(),
                    OrganizationId = organization.Id,
                    Lang = lang
                });
            }

            await _repository.AddOrganization(organization);
            await _repository.AddMultilang(multilang);

            var result = new CreateOrganizationResponse()
            {
                Id = organization.Id
            };

            return result;
        }
    }
}
