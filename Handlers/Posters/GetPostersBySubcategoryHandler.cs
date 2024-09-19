using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
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
        private readonly string _lang;

        public GetPostersBySubcategoryHandler(PosterRepository repository, IHttpContextAccessor accessor)
        {
            _repository = repository;
            _lang = accessor.HttpContext.Items["ClientLang"].ToString().ToLower();
        }

        public async Task<List<PosterResponseModel>> Handle(GetPostersBySubcategoryRequest request, CancellationToken cancellationToken = default) =>
            await _repository.GetListNoTracking(request.startDate, request.endDate, request.subcategoryId, _lang);
    }
}
