using InfoPoster_backend.Models.Organizations;
using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Organizations
{
    public class GetOrganizationsRequest : IRequest<List<GetOrganizationsResponse>>
    {
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public Guid categoryId { get; set; } = Guid.Empty;
        public Guid subcategoryId { get; set; } = Guid.Empty;
    }

    public class GetOrganizationsResponse
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; }
        public string Name { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public Guid SubcategoryId { get; set; }
        public string SubcategoryName { get; set; }
        public string Place { get; set; }
    }

    public class GetOrganizationsHandler : IRequestHandler<GetOrganizationsRequest, List<GetOrganizationsResponse>> 
    {
        private readonly OrganizationRepository _repository;
        public GetOrganizationsHandler(OrganizationRepository organizationRepository)
        {
            _repository = organizationRepository;
        }

        public async Task<List<GetOrganizationsResponse>> Handle(GetOrganizationsRequest request, CancellationToken cancellationToken = default)
        {
            var organizationList = await _repository.GetOrganizationList();
            var result = new List<GetOrganizationsResponse>();
            if (request.categoryId != Guid.Empty && request.subcategoryId != Guid.Empty) 
            {
                var category = await _repository.GetCategory(request.categoryId);
                var subcategory = await _repository.GetSubcategory(request.subcategoryId);

                result = organizationList.Where(o => o.CategoryId == request.categoryId && o.SubcategoryId == request.subcategoryId)
                                             .Select(o => new GetOrganizationsResponse()
                                             {
                                                 CategoryId = o.CategoryId,
                                                 CategoryName = category.Name,
                                                 SubcategoryId = subcategory.Id,
                                                 SubcategoryName = subcategory.Name,
                                                 Id = o.Id,
                                                 Name = o.Name,
                                                 CreatedAt = o.CreatedAt
                                             }).ToList();
            } else if (request.subcategoryId != Guid.Empty)
            {
                var subcategory = await _repository.GetSubcategory(request.subcategoryId);
                var categories = await _repository.GetCategories();

                result = organizationList.Where(o => o.SubcategoryId == request.subcategoryId)
                                             .Select(o => new GetOrganizationsResponse()
                                             {
                                                 CategoryId = o.CategoryId,
                                                 CategoryName = categories.Where(c => c.Id == o.CategoryId).Select(c => c.Name).FirstOrDefault(),
                                                 SubcategoryId = subcategory.Id,
                                                 SubcategoryName = subcategory.Name,
                                                 Id = o.Id,
                                                 Name = o.Name,
                                                 CreatedAt = o.CreatedAt
                                             }).ToList();
            } else if (request.categoryId != Guid.Empty)
            {
                var subcategories = await _repository.GetSubcategories();
                var category = await _repository.GetCategory(request.categoryId);

                result = organizationList.Where(o => o.CategoryId == request.categoryId)
                                             .Select(o => new GetOrganizationsResponse()
                                             {
                                                 CategoryId = o.CategoryId,
                                                 CategoryName = category.Name,
                                                 SubcategoryId = o.SubcategoryId,
                                                 SubcategoryName = subcategories.Where(c => c.Id == o.SubcategoryId).Select(c => c.Name).FirstOrDefault(),
                                                 Id = o.Id,
                                                 Name = o.Name,
                                                 CreatedAt = o.CreatedAt
                                             }).ToList();
            } else
            {
                var categories = await _repository.GetCategories();
                var subcategories = await _repository.GetSubcategories();

                result = organizationList.Select(o => new GetOrganizationsResponse()
                                         {
                                                CategoryId = o.CategoryId,
                                                CategoryName = categories.Where(c => c.Id == o.CategoryId).Select(c => c.Name).FirstOrDefault(),
                                                SubcategoryId = o.SubcategoryId,
                                                SubcategoryName = subcategories.Where(c => c.Id == o.SubcategoryId).Select(c => c.Name).FirstOrDefault(),
                                                Id = o.Id,
                                                Name = o.Name,
                                                CreatedAt = o.CreatedAt
                                         }).ToList();
            }
            return result;
        }

    }
}
