using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Login;
using MediatR;

namespace InfoPoster_backend.Handlers.Statistics
{
    public class GetStatisticActualRequest : IRequest<GetStatisticActualResponse> { }

    public class GetStatisticActualResponse
    {
        public int Draft { get; set; }
        public int Pending { get; set; }
        public int Deleted { get; set; }
        public int Published { get; set; }
        public int Rejected { get; set; }
    }

    public class GetStatisticActualHandler : IRequestHandler<GetStatisticActualRequest, GetStatisticActualResponse>
    {
        private readonly OrganizationRepository _organization;
        private readonly PosterRepository _poster;

        private readonly Guid _user;

        public GetStatisticActualHandler(OrganizationRepository organization, PosterRepository poster, LoginService loginService)
        {
            _organization = organization;
            _poster = poster;
            _user = loginService.GetUserId();
        }

        public async Task<GetStatisticActualResponse> Handle(GetStatisticActualRequest request, CancellationToken cancellationToken = default)
        {
            var rejectedOrgs = await _organization.GetCountByStatus((int)POSTER_STATUS.REJECTED);
            var rejectedPost = await _poster.GetCountByStatus((int)POSTER_STATUS.REJECTED);

            var draftOrgs = await _organization.GetCountByStatus((int)POSTER_STATUS.DRAFT);
            var draftPost = await _poster.GetCountByStatus((int)POSTER_STATUS.DRAFT);

            var deletedOrgs = await _organization.GetCountByStatus((int)POSTER_STATUS.DELETED);
            var deletedPost = await _poster.GetCountByStatus((int)POSTER_STATUS.DELETED);

            var pendingOrgs = await _organization.GetCountByStatus((int)POSTER_STATUS.PENDING);
            var pendingPost = await _poster.GetCountByStatus((int)POSTER_STATUS.PENDING);

            var publishedOrgs = await _organization.GetCountByStatus((int)POSTER_STATUS.PUBLISHED);
            var publishedPost = await _poster.GetCountByStatus((int)POSTER_STATUS.PUBLISHED);

            var result = new GetStatisticActualResponse()
            {
                Draft = draftOrgs + draftPost,
                Pending = pendingOrgs + pendingPost,
                Deleted = deletedOrgs + deletedPost,
                Published = publishedOrgs + publishedPost,
                Rejected = rejectedOrgs + rejectedPost
            };
            return result;
        }
    }
}
