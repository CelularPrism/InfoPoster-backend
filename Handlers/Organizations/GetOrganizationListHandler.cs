using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Login;
using MediatR;

namespace InfoPoster_backend.Handlers.Organizations
{
    public class GetOrganizationListRequest : IRequest<GetOrganizationListResponse>
    {
        public int Sort { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? SubcategoryId { get; set; }
        public Guid? CityId { get; set; }
        public int? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Page { get; set; }
        public int CountPerPage { get; set; }
    }

    public class GetOrganizationListResponse
    {
        public List<OrganizationResponseModel> Organizations { get; set; }
        public int Count { get; set; }
        public int Page { get; set; }
        public int CountPerPage { get; set; }
    }

    public class OrganizationResponseModel
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public Guid SubcategoryId { get; set; }
        public string SubcategoryName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Name { get; set; }
        public int Status { get; set; }
        public Guid? CityId { get; set; }
        public string CityName { get; set; }
    }

    public class GetOrganizationListHandler : IRequestHandler<GetOrganizationListRequest, GetOrganizationListResponse>
    {
        private readonly OrganizationRepository _repository;
        private readonly LoginService _loginService;
        private readonly string _lang;

        public GetOrganizationListHandler(OrganizationRepository repository, LoginService loginService, IHttpContextAccessor accessor)
        {
            _repository = repository;
            _loginService = loginService;
            _lang = accessor.HttpContext.Items["ClientLang"].ToString().ToLower();
        }

        public async Task<GetOrganizationListResponse> Handle(GetOrganizationListRequest request, CancellationToken cancellationToken = default)
        {
            var userId = _loginService.GetUserId();
            var organizations = await _repository.GetOrganizationList(_lang, userId, request.CategoryId, request.Status, request.StartDate, request.EndDate, userId, request.CityId);
            var cities = await _repository.GetCities(_lang);
            var categories = await _repository.GetCategories();
            var subcategories = await _repository.GetSubcategories();
            var result = new GetOrganizationListResponse()
            {
                Count = organizations.Count,
                CountPerPage = request.CountPerPage,
                Page = request.Page + 1
            };

            if (request.Sort == 0)
            {
                organizations = organizations.OrderByDescending(x => x.CreatedAt).ToList();
            }
            else
            {
                organizations = organizations.OrderBy(x => x.Status).ToList();
            }

            organizations = organizations.Skip(request.Page * request.CountPerPage).Take(request.CountPerPage).ToList();
            var idEnum = organizations.Select(x => x.Id);
            var multilang = await _repository.GetMultilang(idEnum);
            var fullInfo = await _repository.GetFullInfo(idEnum);

            var orgList = organizations.Select(o => new OrganizationResponseModel()
            {
                CategoryId = o.CategoryId,
                CategoryName = categories.Where(c => c.Id == o.CategoryId).Select(c => c.Name).FirstOrDefault(),
                CityId = fullInfo.Where(f => f.OrganizationId == o.Id).Select(f => f.City).FirstOrDefault(),
                CityName = fullInfo.Where(f => f.OrganizationId == o.Id)
                                   .Join(cities,
                                         f => f.City,
                                         c => c.Id,
                                         (f, c) => c.Name)
                                   .FirstOrDefault(),
                CreatedAt = o.CreatedAt,
                Id = o.Id,
                Name = multilang.Where(m => m.OrganizationId == o.Id).Select(m => m.Name).FirstOrDefault(),
                Status = o.Status,
                SubcategoryId = o.SubcategoryId,
                SubcategoryName = subcategories.Where(s => s.Id == o.SubcategoryId).Select(s => s.Name).FirstOrDefault()
            }).ToList();

            if (request.Sort == 0)
            {
                orgList = orgList.OrderByDescending(x => x.CreatedAt).ToList();
            }
            else
            {
                orgList = orgList.OrderBy(x => x.Status).ToList();
            }
            result.Organizations = orgList;

            return result;
        }
    }
}
