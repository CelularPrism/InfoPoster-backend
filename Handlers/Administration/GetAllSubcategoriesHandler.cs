using InfoPoster_backend.Models;
using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Administration
{
    public class GetAllSubcategoriesRequest : IRequest<List<SubcategoryModel>>
    {
        public CategoryType type { get; set; }
        public bool IsAdmin { get; set; }
    }

    public class GetAllSubcategoriesHandler : IRequestHandler<GetAllSubcategoriesRequest, List<SubcategoryModel>>
    {
        private readonly CategoryRepository _repository;
        public GetAllSubcategoriesHandler(CategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<SubcategoryModel>> Handle(GetAllSubcategoriesRequest request, CancellationToken cancellationToken = default) =>
            await _repository.GetSubcategories((int)request.type);
    }
}
