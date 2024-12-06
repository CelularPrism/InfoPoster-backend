using InfoPoster_backend.Models.Organizations;
using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Organizations
{
    public class GetAllOrganizationRequest : IRequest<GetAllOrganizationResponse>
    {
        public int Sort { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? CityId { get; set; }
        public int? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? UserId { get; set; }
        public int Page { get; set; }
        public int CountPerPage { get; set; }
    }

    public class GetAllOrganizationResponse
    {
        public List<AllOrganizationModel> Organizations { get; set; }
        public int Count { get; set; }
        public int Page { get; set; }
        public int CountPerPage { get; set; }
    }

    public class AllOrganizationModel
    {
        public AllOrganizationModel(OrganizationModel organization, OrganizationMultilangModel multilang, string userName)
        {
            Id = organization.Id;
            Name = multilang.Name;
            CreatedAt = organization.CreatedAt;
            CategoryId = organization.CategoryId;
            SubategoryId = organization.SubcategoryId;
            UserId = organization.UserId;
            CreatedBy = userName;
            Status = organization.Status;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CategoryId { get; set; }
        public Guid SubategoryId { get; set; }
        public Guid UserId { get; set; }
        public string CreatedBy { get; set; }
        public int Status { get; set; }
        public string CategoryName { get; set; }
        public Guid? CityId { get; set; }
        public string CityName { get; set; }
        public string SubcategoryName { get; set; }
        public Guid? LastUpdatedBy { get; set; }
        public string LastUpdatedByName { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
    }

    public class GetAllOrganizationHandler : IRequestHandler<GetAllOrganizationRequest, GetAllOrganizationResponse>
    {
        private readonly OrganizationRepository _repository;
        private readonly string _lang;

        public GetAllOrganizationHandler(OrganizationRepository repository, IHttpContextAccessor accessor)
        {
            _repository = repository;
            _lang = accessor.HttpContext.Items["ClientLang"].ToString().ToLower();
        }

        public async Task<GetAllOrganizationResponse> Handle(GetAllOrganizationRequest request, CancellationToken cancellationToken = default)
        {
            var organizations = await _repository.GetOrganizationList(_lang);

            if (request.CategoryId != null)
            {
                organizations = organizations.Where(x => x.CategoryId == request.CategoryId).ToList();
            }

            if (request.CityId != null)
            {
                organizations = organizations.Where(x => x.CityId == request.CityId).ToList();
            }

            if (request.Status != null)
            {
                organizations = organizations.Where(x => x.Status == request.Status).ToList();
            }

            if (request.StartDate != null)
            {
                organizations = organizations.Where(x => x.CreatedAt >= request.StartDate).ToList();
            }

            if (request.EndDate != null)
            {
                organizations = organizations.Where(x => x.CreatedAt <= request.EndDate).ToList();
            }

            if (request.UserId != null)
            {
                organizations = organizations.Where(x => x.UserId == request.UserId).ToList();
            }

            if (request.Sort == 0)
            {
                organizations = organizations.OrderByDescending(x => x.CreatedAt).ToList();
            }
            else
            {
                organizations = organizations.OrderBy(x => x.Status).ToList();
            }

            var result = new GetAllOrganizationResponse()
            {
                Count = organizations.Count,
                CountPerPage = request.CountPerPage,
                Page = request.Page
            };

            organizations = organizations.Skip(request.Page * request.CountPerPage).Take(request.CountPerPage).ToList();
            result.Organizations = organizations;
            return result;
        }
    }
}
