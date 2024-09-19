using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Posters
{
    public class GetPostersByCategoryRequest : IRequest<List<PosterResponseModel>>
    {
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public Guid categoryId { get; set; }
    }

    public class GetPostersByCategoryHandler : IRequestHandler<GetPostersByCategoryRequest, List<PosterResponseModel>>
    {
        private readonly PosterRepository _repository;
        private readonly string _lang;

        public GetPostersByCategoryHandler(PosterRepository repository, IHttpContextAccessor accessor)
        {
            _repository = repository;
            _lang = accessor.HttpContext.Items["ClientLang"].ToString().ToLower();
        }

        public async Task<List<PosterResponseModel>> Handle(GetPostersByCategoryRequest request, CancellationToken cancellationToken = default) => 
            await _repository.GetListNoTracking(request.startDate, request.endDate, request.categoryId, _lang);
    }
}
