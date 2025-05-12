using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Models.Selectel;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Selectel_API;
using MediatR;

namespace InfoPoster_backend.Handlers.Posters
{
    public class GetUpcomingPostersRequest : IRequest<List<PosterResponseModel>> { }

    public class GetUpcomingPostersHandler : IRequestHandler<GetUpcomingPostersRequest, List<PosterResponseModel>>
    {
        private readonly PosterRepository _repository;
        private readonly SelectelAuthService _selectel;
        private readonly string _lang;

        public GetUpcomingPostersHandler(PosterRepository repository, SelectelAuthService selectel, IHttpContextAccessor accessor)
        {
            _repository = repository;
            _selectel = selectel;
            _lang = accessor.HttpContext.Items["ClientLang"].ToString().ToLower();
        }

        public async Task<List<PosterResponseModel>> Handle(GetUpcomingPostersRequest request, CancellationToken cancellationToken = default)
        {
            var authTask = _selectel.Login();
            var posters = await _repository.GetListNoTracking(DateTime.UtcNow, DateTime.Today.AddYears(1), 9, _lang);

            var isLoggedIn = await authTask;

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
