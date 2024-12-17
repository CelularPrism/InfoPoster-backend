using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Statistics
{
    public class GetCommonStatRequest : IRequest<List<GetCommonStatResponse>>
    {
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
    }

    public class GetCommonStatResponse
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string ImageSrc { get; set; }
        public int CountDeleted { get; set; }
        public int CountDraft { get; set; }
        public int CountPending { get; set; }
        public int CountPublished { get; set; }
    }

    public class GetCommonStatHandler : IRequestHandler<GetCommonStatRequest, List<GetCommonStatResponse>>
    {
        private readonly StatisticRepository _repository;

        public GetCommonStatHandler(StatisticRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<GetCommonStatResponse>> Handle(GetCommonStatRequest request, CancellationToken cancellationToken)
        {
            var organizations = await _repository.GetOrganizationList(request.DateStart, request.DateEnd);
            var posters = await _repository.GetPosterList(request.DateStart, request.DateEnd);
            var users = await _repository.GetUserList();

            var result = users.Where(us => !us.IsBlocked).Select(us => new GetCommonStatResponse() 
            {
                UserId = us.Id,
                UserName = us.FirstName + " " + us.LastName + " (" + us.Email + ")",
                ImageSrc = us.ImageSrc,
                CountDeleted = organizations.Where(o => o.Status == (int)POSTER_STATUS.DELETED && o.UserId == us.Id).Count() + posters.Where(o => o.Status == (int)POSTER_STATUS.DELETED && o.UserId == us.Id).Count(),
                CountDraft = organizations.Where(o => o.Status == (int)POSTER_STATUS.DRAFT && o.UserId == us.Id).Count() + posters.Where(o => o.Status == (int)POSTER_STATUS.DRAFT && o.UserId == us.Id).Count(),
                CountPending = organizations.Where(o => o.Status == (int)POSTER_STATUS.PENDING && o.UserId == us.Id).Count() + posters.Where(o => o.Status == (int)POSTER_STATUS.PENDING && o.UserId == us.Id).Count(),
                CountPublished = organizations.Where(o => o.Status == (int)POSTER_STATUS.PUBLISHED && o.UserId == us.Id).Count() + posters.Where(o => o.Status == (int)POSTER_STATUS.PUBLISHED && o.UserId == us.Id).Count(),
            }).ToList();

            return result;
        }
    }
}
