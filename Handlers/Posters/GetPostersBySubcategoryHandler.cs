using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Models.Selectel;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Selectel_API;
using MediatR;

namespace InfoPoster_backend.Handlers.Posters
{
    public class GetPostersBySubcategoryRequest : IRequest<List<PosterResponseModel>>
    {
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public Guid subcategoryId { get; set; }
    }

    public class GetPostersBySubcategoryHandler : IRequestHandler<GetPostersBySubcategoryRequest, List<PosterResponseModel>>
    {
        private readonly PosterRepository _repository;
        private readonly SelectelAuthService _selectelAuth;
        private readonly string _lang;

        public GetPostersBySubcategoryHandler(PosterRepository repository, IHttpContextAccessor accessor, SelectelAuthService selectelAuth)
        {
            _repository = repository;
            _lang = accessor.HttpContext.Items["ClientLang"].ToString().ToLower();
            _selectelAuth = selectelAuth;
        }

        public async Task<List<PosterResponseModel>> Handle(GetPostersBySubcategoryRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _repository.GetListNoTracking(request.startDate, request.endDate, request.subcategoryId, _lang);

            var loggedIn = await _selectelAuth.Login();
            var selectelUUID = string.Empty;
            if (loggedIn)
            {
                selectelUUID = await _selectelAuth.GetContainerUUID("dosdoc");
                foreach (var poster in result)
                {
                    if (poster.FileId != null || poster.FileId != Guid.Empty)
                        poster.FileURL = string.Concat("https://", selectelUUID, ".selstorage.ru/", poster.FileId);
                }
            }
            return result;
        }
    }
}
