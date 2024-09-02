using InfoPoster_backend.Models;
using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Posters
{
    public class GetCategoriesRequest : IRequest<List<CategoryModel>> { }

    public class GetCategoriesHandler : IRequestHandler<GetCategoriesRequest, List<CategoryModel>> 
    {
        private readonly CategoryRepository _repository;
        
        public GetCategoriesHandler(CategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<CategoryModel>> Handle(GetCategoriesRequest request, CancellationToken cancellationToken = default) =>
            await _repository.GetCategoriesNoTracking();
    }
}
