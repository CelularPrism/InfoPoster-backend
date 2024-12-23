using InfoPoster_backend.Models.Organizations;
using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Login;
using InfoPoster_backend.Tools;
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
        public AllOrganizationModel() { }

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
    }

    public class GetAllOrganizationHandler : IRequestHandler<GetAllOrganizationRequest, GetAllOrganizationResponse>
    {
        private readonly OrganizationRepository _repository;
        private readonly AccountRepository _accountRepository;
        private readonly string _lang;
        private Guid _user;

        public GetAllOrganizationHandler(OrganizationRepository repository, AccountRepository accountRepository, IHttpContextAccessor accessor, LoginService loginService)
        {
            _repository = repository;
            _accountRepository = accountRepository;
            _lang = accessor.HttpContext.Items["ClientLang"].ToString().ToLower();
            _user = loginService.GetUserId();
        }

        public async Task<GetAllOrganizationResponse> Handle(GetAllOrganizationRequest request, CancellationToken cancellationToken = default)
        {
            var availableStatuses = new List<int>()
            {
                (int)POSTER_STATUS.PENDING,
                (int)POSTER_STATUS.PUBLISHED,
                (int)POSTER_STATUS.DRAFT,
                (int)POSTER_STATUS.REJECTED,
                (int)POSTER_STATUS.REVIEWING
            };

            if (request.Status != null)
            {
                availableStatuses = new List<int>()
                {
                    (int)request.Status
                };
            }

            var organizations = await _repository.GetOrganizationList(_lang, _user, availableStatuses, request.CategoryId, request.StartDate, request.EndDate, request.UserId, request.CityId);
            var cities = await _repository.GetCities(_lang);
            var categories = await _repository.GetCategories();
            var subcategories = await _repository.GetSubcategories();
            var result = new GetAllOrganizationResponse()
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
            var userEnum = organizations.Select(x => x.UserId);
            var multilang = await _repository.GetMultilang(idEnum);
            var fullInfo = await _repository.GetFullInfo(idEnum);
            var users = await _repository.GetUsers(userEnum);

            var orgList = organizations.Select(o => new AllOrganizationModel()
            {
                Id = o.Id,
                CategoryId = o.CategoryId,
                CategoryName = categories.Where(c => c.Id == o.CategoryId).Select(c => c.Name).FirstOrDefault(),
                Name = multilang.Where(m => m.OrganizationId == o.Id).Select(m => m.Name).FirstOrDefault(),
                CityId = fullInfo.Where(f => f.OrganizationId == o.Id).Select(f => f.City).FirstOrDefault(),
                CityName = fullInfo.Where(f => f.OrganizationId == o.Id)
                                   .Join(cities,
                                         f => f.City,
                                         c => c.Id,
                                         (f, c) => c.Name)
                                   .FirstOrDefault(),
                CreatedAt = o.CreatedAt,
                CreatedBy = users.Where(u => u.Id == o.UserId).Select(u => u.FirstName + " " + u.LastName).FirstOrDefault(),
                UserId = o.UserId,
                Status = o.Status,
                SubategoryId = o.SubcategoryId,
                SubcategoryName = subcategories.Where(s => s.Id == o.SubcategoryId).Select(s => s.Name).FirstOrDefault(),
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
