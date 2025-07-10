using InfoPoster_backend.Handlers.Organizations;
using InfoPoster_backend.Models.Administration;
using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Selectel_API;
using MediatR;

namespace InfoPoster_backend.Handlers.Posters
{
    public class GetPopularPostersRequest : IRequest<List<PosterResponseModel>>
    {
        public POPULARITY_PLACE Place { get; set; }
        public Guid? SubcategoryId { get; set; } = null;
    }

    public class GetPopularPostersHandler : IRequestHandler<GetPopularPostersRequest, List<PosterResponseModel>>
    {
        private readonly PosterRepository _repository;
        private readonly FileRepository _file;
        private readonly SelectelAuthService _selectel;

        public GetPopularPostersHandler(PosterRepository repository, FileRepository file, SelectelAuthService selectel)
        {
            _repository = repository;
            _file = file;
            _selectel = selectel;
        }

        public async Task<List<PosterResponseModel>> Handle(GetPopularPostersRequest request, CancellationToken cancellationToken = default)
        {
            //var logs = new List<PosterViewLogModel>();
            //var result = new List<PosterResponseModel>();

            //if (request.SubcategoryId != null && request.SubcategoryId != Guid.Empty)
            //{
            //    logs = await _repository.GetPublishedViewLogsBySubcategory(DateTime.UtcNow.Date, DateTime.UtcNow.AddYears(1), (Guid)request.SubcategoryId);
            //} else
            //{
            //    logs = await _repository.GetPublishedViewLogs(DateTime.UtcNow.Date, DateTime.UtcNow.AddYears(1));
            //}

            //var popularPosters = logs.GroupBy(l => l.PosterId).Select(l => new
            //{
            //    PosterId = l.Key,
            //    Count = l.Count()
            //}).OrderByDescending(l => l.Count).Take(9).Select(l => l.PosterId).ToList();

            //var posters = await _repository.GetPosters(popularPosters);

            var result = await _repository.GetPopularPosterList(request.Place);
            var isLoggedIn = await _selectel.Login();

            if (isLoggedIn)
            {
                var selectelUUID = await _selectel.GetContainerUUID("dosdoc");
                foreach (var item in result)
                {
                    var file = await _file.GetPrimaryFile(item.Id, 0);
                    if (file == null)
                        file = await _file.GetApplicationFileByApplication(item.Id);

                    item.FileURL = string.Concat("https://", selectelUUID, ".selstorage.ru/", file.FileId);
                }
            }
            return result;
        }
    }
}
