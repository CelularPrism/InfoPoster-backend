using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Login;
using MediatR;

namespace InfoPoster_backend.Handlers.Statistics
{
    public class GetStatisticActualByEditorRequest : IRequest<GetStatisticActualByEditorResponse> { }

    public class GetStatisticActualByEditorResponse
    {
        public int Draft {  get; set; }
        public int Pending {  get; set; }
        public int Deleted {  get; set; }
        public int Published {  get; set; }
    }

    public class GetStatisticActualByEditorHandler : IRequestHandler<GetStatisticActualByEditorRequest, GetStatisticActualByEditorResponse>
    {
        private readonly OrganizationRepository _organization;
        private readonly PosterRepository _poster;

        private readonly Guid _user;

        public GetStatisticActualByEditorHandler(OrganizationRepository organization, PosterRepository poster, LoginService loginService)
        {
            _organization = organization;
            _poster = poster;
            _user = loginService.GetUserId();
        }

        public async Task<GetStatisticActualByEditorResponse> Handle(GetStatisticActualByEditorRequest request, CancellationToken cancellationToken = default)
        {
            var organizations = await _organization.GetOrganizationListByUserId(_user);
            var posters = await _poster.GetPosterListByUserId(_user);

            var result = new GetStatisticActualByEditorResponse()
            {
                Draft = organizations.Count(o => o.Status == (int)POSTER_STATUS.DRAFT) + posters.Count(p => p.Status == (int)POSTER_STATUS.DRAFT),
                Pending = organizations.Count(o => o.Status == (int)POSTER_STATUS.PENDING) + posters.Count(p => p.Status == (int)POSTER_STATUS.PENDING),
                Deleted = organizations.Count(o => o.Status == (int)POSTER_STATUS.DELETED) + posters.Count(p => p.Status == (int)POSTER_STATUS.DELETED),
                Published = organizations.Count(o => o.Status == (int)POSTER_STATUS.PUBLISHED) + posters.Count(p => p.Status == (int)POSTER_STATUS.PUBLISHED),
            };
            return result;
        }
    }
}
