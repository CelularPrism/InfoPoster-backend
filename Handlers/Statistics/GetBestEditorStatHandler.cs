using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InfoPoster_backend.Handlers.Statistics
{
    public class GetBestEditorStatRequest : IRequest<List<GetBestEditorStatResponse>>
    {
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }
    }

    public class GetBestEditorStatResponse
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string ImageSrc { get; set; }
        public int CountDraft { get; set; }
    }

    public class GetBestEditorStatHandler : IRequestHandler<GetBestEditorStatRequest, List<GetBestEditorStatResponse>>
    {
        private readonly StatisticRepository _repository;

        public GetBestEditorStatHandler(StatisticRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<GetBestEditorStatResponse>> Handle(GetBestEditorStatRequest request, CancellationToken cancellationToken)
        {
            var organizations = await _repository.GetOrganizationList(request.DateStart, request.DateEnd);
            var posters = await _repository.GetPosterList(request.DateStart, request.DateEnd);
            var users = await _repository.GetUserList();

            var orgList = organizations.Where(o => o.Status == (int)POSTER_STATUS.DRAFT)
                                       .GroupBy(o => o.UserId)
                                       .Select(o => new
                                       {
                                           UserId = o.Key,
                                           Count = o.Count()
                                       }).ToList();

            var posterList = posters.Where(p => p.Status == (int)POSTER_STATUS.DRAFT)
                                    .GroupBy(p => p.UserId)
                                    .Select(p => new
                                    {
                                        UserId = p.Key,
                                        Count = p.Count()
                                    }).ToList();

            var result = users.Select(u => new GetBestEditorStatResponse()
            {
                UserId = u.Id,
                UserName = u.FirstName + " " + u.LastName,
                Email = u.Email,
                ImageSrc = u.ImageSrc,
                CountDraft = orgList.Where(org => org.UserId == u.Id).Select(org => org.Count).FirstOrDefault() + posterList.Where(p => p.UserId == u.Id).Select(p => p.Count).FirstOrDefault()
            }).OrderByDescending(u => u.CountDraft).Take(3).ToList();

            return result;
        }
    }
}
