using InfoPoster_backend.Models;
using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Account
{
    public class SearchApplicationRequest : IRequest<List<SearchApplicationResponse>>
    {
        public string SearchText { get; set; }
    }

    public class SearchApplicationResponse
    {
        public Guid Id { get; set; }
        public SEARCH_TYPE Type { get; set; }
        public string Name { get; set; }
    }

    public class SearchApplicationHandler : IRequestHandler<SearchApplicationRequest, List<SearchApplicationResponse>>
    {
        private readonly PosterRepository _posterRepository;
        private readonly OrganizationRepository _organizationRepository;
        private readonly CategoryRepository _categoryRepository;

        public SearchApplicationHandler(PosterRepository posterRepository, OrganizationRepository organizationRepository, CategoryRepository categoryRepository)
        {
            _posterRepository = posterRepository;
            _organizationRepository = organizationRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<List<SearchApplicationResponse>> Handle(SearchApplicationRequest request, CancellationToken cancellationToken = default)
        {
            var categories = await _categoryRepository.SearchCategories(request.SearchText);
            var result = new List<SearchApplicationResponse>();

            foreach (var category in categories)
            {
                result.Add(new SearchApplicationResponse()
                {
                    Id = category.Id,
                    Name = category.Name,
                    Type = category.Type == (int)CategoryType.PLACE ? SEARCH_TYPE.CATEGORY_PLACE : SEARCH_TYPE.CATEGORY_EVENT,
                });
            }
            
            if (result.Count >= 5)
                return result;

            var subcategories = await _categoryRepository.SearchSubcategories(request.SearchText);

            foreach (var subcategory in subcategories)
            {
                result.Add(new SearchApplicationResponse()
                {
                    Id = subcategory.Id,
                    Name = subcategory.Name,
                    Type = SEARCH_TYPE.SUBCATEGORY
                });

                if (result.Count >= 5)
                    return result;
            }

            var organizations = await _organizationRepository.SearchOrganizations(request.SearchText);

            foreach (var organization in organizations)
            {
                result.Add(new SearchApplicationResponse()
                {
                    Id = organization.OrganizationId,
                    Name = organization.Name,
                    Type = SEARCH_TYPE.PLACE
                });

                if (result.Count >= 5)
                    return result;
            }

            var posters = await _posterRepository.SearchPosters(request.SearchText);

            foreach (var poster in posters)
            {
                result.Add(new SearchApplicationResponse()
                {
                    Id = poster.Id,
                    Name = poster.Name,
                    Type = SEARCH_TYPE.EVENT
                });

                if (result.Count >= 5)
                    return result;
            }

            return result;
        }
    }
}
