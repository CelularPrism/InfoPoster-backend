using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Selectel_API;
using MediatR;

namespace InfoPoster_backend.Handlers.Posters
{
    public class GetPopularPostersRequest : IRequest<List<PosterResponseModel>> 
    {
        public Guid? SubcategoryId { get; set; } = null;
    }

    public class GetPopularPostersHandler : IRequestHandler<GetPopularPostersRequest, List<PosterResponseModel>>
    {
        private readonly PosterRepository _repository;
        private readonly SelectelAuthService _selectel;

        public GetPopularPostersHandler(PosterRepository repository, SelectelAuthService selectel)
        {
            _repository = repository;
            _selectel = selectel;
        }

        public async Task<List<PosterResponseModel>> Handle(GetPopularPostersRequest request, CancellationToken cancellationToken = default)
        {
            var logs = new List<PosterViewLogModel>();

            if (request.SubcategoryId != null && request.SubcategoryId != Guid.Empty)
            {
                logs = await _repository.GetPublishedViewLogsBySubcategory(DateTime.UtcNow.Date, DateTime.UtcNow.AddYears(1), (Guid)request.SubcategoryId);
            } else
            {
                logs = await _repository.GetPublishedViewLogs(DateTime.UtcNow.Date, DateTime.UtcNow.AddYears(1));
            }

            var popularPosters = logs.GroupBy(l => l.PosterId).Select(l => new
            {
                PosterId = l.Key,
                Count = l.Count()
            }).OrderByDescending(l => l.Count).Take(9).Select(l => l.PosterId).ToList();

            var posters = await _repository.GetPosters(popularPosters);

            var isLoggedIn = await _selectel.Login();

            if (isLoggedIn)
            {
                var selectelUUID = await _selectel.GetContainerUUID("dosdoc");
                foreach (var item in posters)
                {
                    item.FileURL = string.Concat("https://", selectelUUID, ".selstorage.ru/", item.FileId);
                }
            }
            return posters;
        }
    }
}
