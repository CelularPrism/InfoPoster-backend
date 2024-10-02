using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Login;
using MediatR;

namespace InfoPoster_backend.Handlers.Organizations
{
    public class GetOrganizationListRequest : IRequest<List<GetOrganizationListResponse>> { }

    public class GetOrganizationListResponse
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Name { get; set; }
    }

    public class GetOrganizationListHandler : IRequestHandler<GetOrganizationListRequest, List<GetOrganizationListResponse>>
    {
        private readonly OrganizationRepository _repository;
        private readonly LoginService _loginService;

        public GetOrganizationListHandler(OrganizationRepository repository, LoginService loginService)
        {
            _repository = repository;
            _loginService = loginService;
        }

        public async Task<List<GetOrganizationListResponse>> Handle(GetOrganizationListRequest request, CancellationToken cancellationToken = default)
        {
            var userId = _loginService.GetUserId();
            var organizations = await _repository.GetOrganizationList(userId);
            var result = organizations.Select(o => new GetOrganizationListResponse()
            {
                Id = o.Id,
                CreatedAt = o.CreatedAt,
                Name = o.Name
            }).ToList();

            return result;
        }
    }
}
