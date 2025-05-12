using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Models.Selectel;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Selectel_API;
using MediatR;

namespace InfoPoster_backend.Handlers.Posters
{
    public class GetPostersRequest : IRequest<List<PosterResponseModel>> 
    {
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
    }

    public class GetPostersHandler : IRequestHandler<GetPostersRequest, List<PosterResponseModel>>
    {
        private readonly PosterRepository _repository;
        private readonly SelectelAuthService _selectelAuth;
        private readonly string _lang;

        public GetPostersHandler(PosterRepository repository, SelectelAuthService selectelAuth, IHttpContextAccessor accessor)
        {
            _repository = repository;
            _selectelAuth = selectelAuth;
            _lang = accessor.HttpContext.Items["ClientLang"].ToString().ToLower();
        }

        public async Task<List<PosterResponseModel>> Handle(GetPostersRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _repository.GetListNoTracking(request.startDate, request.endDate, _lang);

            var loggedIn = await _selectelAuth.Login();
            if (loggedIn)
            {
                var selectelUUID = await _selectelAuth.GetContainerUUID("dosdoc");
                foreach (var item in result)
                {
                    item.FileURL = string.Concat("https://", selectelUUID, ".selstorage.ru/", item.FileId);
                }
            }

            return result;
        }
    }
}
