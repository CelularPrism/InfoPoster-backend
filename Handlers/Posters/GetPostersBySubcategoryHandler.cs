using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Models.Selectel;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Selectel_API;
using MediatR;

namespace InfoPoster_backend.Handlers.Posters
{
    public class GetPostersBySubcategoryRequest : IRequest<GetPostersBySubcategoryResponse>
    {
        public DateTime startDate { get; set; } = DateTime.UtcNow;
        public DateTime endDate { get; set; } = DateTime.UtcNow.AddYears(1);
        public Guid subcategoryId { get; set; }
        public int Limit { get; set; } = 9;
        public int Offset { get; set; } = 0;
    }

    public class GetPostersBySubcategoryResponse
    {
        public List<PosterResponseModel> data { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public int Total { get; set; }
    }

    public class GetPostersBySubcategoryHandler : IRequestHandler<GetPostersBySubcategoryRequest, GetPostersBySubcategoryResponse>
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

        public async Task<GetPostersBySubcategoryResponse> Handle(GetPostersBySubcategoryRequest request, CancellationToken cancellationToken = default)
        {
            var (list, total) = await _repository.GetListNoTracking(request.Limit, request.Offset, request.startDate, request.endDate, request.subcategoryId, _lang);

            var loggedIn = await _selectelAuth.Login();
            var selectelUUID = string.Empty;
            if (loggedIn)
            {
                selectelUUID = await _selectelAuth.GetContainerUUID("dosdoc");
                foreach (var poster in list)
                {
                    if (poster.FileId != null || poster.FileId != Guid.Empty)
                        poster.FileURL = string.Concat("https://", selectelUUID, ".selstorage.ru/", poster.FileId);
                }
            }

            var result = new GetPostersBySubcategoryResponse()
            {
                data = list,
                Offset = request.Offset,
                Limit = request.Limit,
                Total = total
            };

            return result;
        }
    }
}
