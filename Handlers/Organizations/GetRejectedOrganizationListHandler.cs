using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Login;
using MediatR;

namespace InfoPoster_backend.Handlers.Organizations
{
    public class GetRejectedOrganizationListRequest : IRequest<GetOrganizationListResponse>
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

    public class GetRejectedOrganizationListHandler : IRequestHandler<GetRejectedOrganizationListRequest, GetOrganizationListResponse>
    {
        private readonly OrganizationRepository _repository;
        private readonly LoginService _loginService;
        private readonly string _lang;

        public GetRejectedOrganizationListHandler(OrganizationRepository repository, LoginService loginService, IHttpContextAccessor accessor)
        {
            _repository = repository;
            _loginService = loginService;
            _lang = accessor.HttpContext.Items["ClientLang"].ToString().ToLower();
        }

        public async Task<GetOrganizationListResponse> Handle(GetRejectedOrganizationListRequest request, CancellationToken cancellationToken = default)
        {
            var categsTask = _repository.GetApplicationCategories();
            var userId = _loginService.GetUserId();
            var organizations = await _repository.GetRejectedOrganizationList(_lang, userId, request.CategoryId, request.Status, request.StartDate, request.EndDate, userId, request.CityId);
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
            var categs = await categsTask;

            var orgList = organizations.Select(o => new OrganizationResponseModel()
            {
                Category = categs.Where(c => c.ApplicationId == o.Id).GroupBy(c => c.CategoryId).Select(g => new Models.IdNameModel()
                {
                    Id = g.Key,
                    Name = categories.Where(c => c.Id == g.Key).Select(c => c.Name).FirstOrDefault()
                }).OrderBy(c => c.Name).ToList(),
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
                Subcategory = categs.Where(c => c.ApplicationId == o.Id).GroupBy(c => c.SubcategoryId).Select(g => new Models.IdNameModel()
                {
                    Id = g.Key,
                    Name = subcategories.Where(c => c.Id == g.Key).Select(c => c.Name).FirstOrDefault()
                }).OrderBy(c => c.Name).ToList(),
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
