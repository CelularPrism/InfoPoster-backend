using InfoPoster_backend.Models;
using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Posters
{
    public class GetSubcategoriesRequest : IRequest<SubcategoryResponseModel>
    {
        public Guid categoryId { get; set; }
    }

    public class GetSubcategoriesHandler : IRequestHandler<GetSubcategoriesRequest, SubcategoryResponseModel>
    {
        private readonly CategoryRepository _repository;
        private readonly string _lang;

        public GetSubcategoriesHandler(CategoryRepository repository, IHttpContextAccessor accessor)
        {
            _repository = repository;
            _lang = accessor.HttpContext.Items["ClientLang"].ToString().ToLower();
        }

        public async Task<SubcategoryResponseModel> Handle(GetSubcategoriesRequest request, CancellationToken cancellationToken = default) =>
            await _repository.GetSubcategoriesNoTracking(request.categoryId, _lang);
    }
}
