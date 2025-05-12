using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Selectel_API;
using MediatR;

namespace InfoPoster_backend.Handlers.Posters
{
    public class GetRecentlyAddedPostersRequest : IRequest<List<PosterResponseModel>> { }

    public class GetRecentlyAddedPostersHandler : IRequestHandler<GetRecentlyAddedPostersRequest, List<PosterResponseModel>>
    {
        private readonly PosterRepository _repository;
        private readonly SelectelAuthService _selectel;

        public GetRecentlyAddedPostersHandler(PosterRepository repository, SelectelAuthService selectel)
        {
            _repository = repository;
            _selectel = selectel;
        }

        public async Task<List<PosterResponseModel>> Handle(GetRecentlyAddedPostersRequest request, CancellationToken cancellationToken = default)
        {
            var history = await _repository.GetStatusHistory(DateTime.UtcNow.AddMonths(-1).Date, POSTER_STATUS.PUBLISHED.ToString());
            var list = history.Select(h => h.ApplicationId).ToList();
            var posters = await _repository.GetPosters(list);

            var result = posters.Take(9).ToList();

            var isLoggedIn = await _selectel.Login();

            if (isLoggedIn)
            {
                var selectelUUID = await _selectel.GetContainerUUID("dosdoc");
                foreach (var item in result)
                {
                    item.FileURL = string.Concat("https://", selectelUUID, ".selstorage.ru/", item.FileId);
                }
            }
            return result;
        }
    }
}
