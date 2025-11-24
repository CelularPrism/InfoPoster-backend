using InfoPoster_backend.Repos;
using InfoPoster_backend.Tools;
using MediatR;

namespace InfoPoster_backend.Handlers.Administration.Poster
{
    public class GetPopularCategoriesRequest : IRequest<List<GetPopularCategoriesResponse>> { }

    public class GetPopularCategoriesResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
    }

    public class GetPopularCategoriesHandler : IRequestHandler<GetPopularCategoriesRequest, List<GetPopularCategoriesResponse>>
    {
        private readonly PosterRepository _repository;
        private readonly Guid _city;
        public GetPopularCategoriesHandler(PosterRepository repository, IHttpContextAccessor accessor)
        {
            _repository = repository;
            _city = Guid.TryParse(accessor.HttpContext.Request.Headers["X-Testing"].ToString(), out _city) ? Guid.Parse(accessor.HttpContext.Request.Headers["X-Testing"].ToString()) : Constants.DefaultCity;
        }

        public async Task<List<GetPopularCategoriesResponse>> Handle(GetPopularCategoriesRequest request, CancellationToken cancellationToken = default)
        {
            var categories = await _repository.GetPopularCategories(_city);

            var result = categories.Select(c => new GetPopularCategoriesResponse()
            {
                Id = c.Id,
                Title = c.Name,
            }).ToList();

            return result;
        }
    }
}
