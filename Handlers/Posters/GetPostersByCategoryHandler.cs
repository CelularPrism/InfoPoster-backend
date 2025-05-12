using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Selectel_API;
using MediatR;

namespace InfoPoster_backend.Handlers.Posters
{
    public class GetPostersByCategoryRequest : IRequest<GetPostersByCategoryResponse>
    {
        public DateTime startDate { get; set; } = DateTime.UtcNow;
        public DateTime endDate { get; set; }
        public Guid categoryId { get; set; }
        public int Limit { get; set; } = 10;
        public int Offset { get; set; } = 0;
    }

    public class GetPostersByCategoryResponse
    {
        public List<PosterResponseModel> data { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public int Total { get; set; }
    }

    public class GetPostersByCategoryHandler : IRequestHandler<GetPostersByCategoryRequest, GetPostersByCategoryResponse>
    {
        private readonly PosterRepository _repository;
        private readonly SelectelAuthService _selectelAuth;
        private readonly string _lang;

        public GetPostersByCategoryHandler(PosterRepository repository, IHttpContextAccessor accessor, SelectelAuthService selectelAuth)
        {
            _repository = repository;
            _lang = accessor.HttpContext.Items["ClientLang"].ToString().ToLower();
            _selectelAuth = selectelAuth;
        }

        public async Task<GetPostersByCategoryResponse> Handle(GetPostersByCategoryRequest request, CancellationToken cancellationToken = default)
        {
            var (list, total) = await _repository.GetListNoTracking(request.Limit, request.Offset, request.startDate, request.endDate, request.categoryId, _lang);

            var loggedIn = await _selectelAuth.Login();
            var selectelUUID = string.Empty;
            if (loggedIn)
            {
                selectelUUID = await _selectelAuth.GetContainerUUID("dosdoc");
                foreach (var poster in list)
                {
                    poster.FileURL = string.Concat("https://", selectelUUID, ".selstorage.ru/", poster.FileId);
                }
            }

            var result = new GetPostersByCategoryResponse()
            {
                Offset = request.Offset,
                Limit = request.Limit,
                data = list,
                Total = total
            };

            return result;
        }
    }
}
