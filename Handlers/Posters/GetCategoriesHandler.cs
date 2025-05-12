using InfoPoster_backend.Models;
using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Posters
{
    public class GetCategoriesRequest : IRequest<List<CategoryResponseModel>>
    {
        public CategoryType type { get; set; }
        public bool IsAdmin { get; set; } = false;
    }

    public class GetCategoriesHandler : IRequestHandler<GetCategoriesRequest, List<CategoryResponseModel>>
    {
        private readonly CategoryRepository _repository;
        private readonly string _lang;

        public GetCategoriesHandler(CategoryRepository repository, IHttpContextAccessor accessor)
        {
            _repository = repository;
            _lang = accessor.HttpContext.Items["ClientLang"].ToString().ToLower();
        }

        public async Task<List<CategoryResponseModel>> Handle(GetCategoriesRequest request, CancellationToken cancellationToken = default) =>
            await _repository.GetCategoriesNoTracking(request.type, _lang, request.IsAdmin);
    }
}
