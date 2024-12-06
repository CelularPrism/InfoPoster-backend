using InfoPoster_backend.Models.Account;
using InfoPoster_backend.Models.Organizations;
using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Statistics
{
    public class GetPublishedRequest : IRequest<GetPublishedResponse>
    {
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
        public Guid? UserId { get; set; }
        public int Status { get; set; }
    }

    public class GetPublishedResponse
    {
        public GetPublishedResponse(List<OrganizationModel> organizations, List<PosterModel> posters, List<UserModel> users, int status)
        {
            Organizations = organizations.Where(o => o.Status == status)
                .GroupBy(o => new { o.UserId, o.CreatedAt })
                .Select(g => new StatisticModel()
                {
                    Date = g.Key.CreatedAt.Date,
                    Count = g.Count()
                }).OrderBy(o => o.Date).ToList();

            Posters = posters.Where(p => p.Status == status)
                .GroupBy(p => new { p.UserId, p.CreatedAt })
                .Select(g => new StatisticModel()
                {
                    Date = g.Key.CreatedAt.Date,
                    Count = g.Count()
                }).OrderBy(p => p.Date).ToList();
        }

        public List<StatisticModel> Organizations { get; set; }
        public List<StatisticModel> Posters { get; set; }
    }

    public class StatisticModel
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }

    public class GetPublishedHandler : IRequestHandler<GetPublishedRequest, GetPublishedResponse>
    {
        private readonly StatisticRepository _repository;
        public GetPublishedHandler(StatisticRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetPublishedResponse> Handle(GetPublishedRequest request, CancellationToken cancellationToken)
        {
            var organizations = await _repository.GetOrganizationList(request.DateStart, request.DateEnd);
            var posters = await _repository.GetPosterList(request.DateStart, request.DateEnd);
            var users = await _repository.GetUserList();
            
            var anyUser = await _repository.AnyUser(request.UserId);
            if (anyUser)
            {
                organizations = organizations.Where(o => o.UserId == request.UserId).ToList();
                posters = posters.Where(p => p.UserId == request.UserId).ToList();
            }

            var result = new GetPublishedResponse(organizations, posters, users, request.Status);
            return result;
        }
    }
}
