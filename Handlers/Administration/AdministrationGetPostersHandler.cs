using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Login;
using InfoPoster_backend.Tools;
using MediatR;
using System.Text.Json.Serialization;

namespace InfoPoster_backend.Handlers.Administration
{
    public class AdministrationGetPostersRequest : IRequest<AdministrationGetPostersResponse>
    {
        public int Sort { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? CityId { get; set; }
        public int? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Page { get; set; }
        public int CountPerPage { get; set; }
    }

    public class AdministrationGetPostersResponse
    {
        public List<AdministrationPostersResponse> Posters { get; set; }
        public int Count { get; set; }
        public int Page { get; set; }
        public int CountPerPage { get; set; }
    }

    public class AdministrationPostersResponse
    {
        public AdministrationPostersResponse() { }

        public AdministrationPostersResponse(PosterModel poster, PosterMultilangModel multilang, string userName)
        {
            Id = poster.Id;
            Name = multilang.Name;
            ReleaseDate = poster.ReleaseDate;
            CreatedAt = poster.CreatedAt;
            UpdatedAt = poster.UpdatedAt;
            CategoryId = poster.CategoryId;
            CreatedBy = userName;
            Status = poster.Status;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        [JsonConverter(typeof(OnlyDateConverter))]
        public DateTime? ReleaseDate { get; set; }
        public Guid CategoryId { get; set; }
        public string CreatedBy { get; set; }
        public int Status { get; set; }
        public string CategoryName { get; set; }
        public Guid? CityId { get; set; }
        public string CityName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class AdministrationGetPostersHandler : IRequestHandler<AdministrationGetPostersRequest, AdministrationGetPostersResponse>
    {
        private readonly LoginService _loginService;
        private readonly PosterRepository _repository;
        private readonly string _lang;

        public AdministrationGetPostersHandler(LoginService loginService, PosterRepository repository, IHttpContextAccessor accessor)
        {
            _loginService = loginService;
            _repository = repository;
            _lang = accessor.HttpContext.Items["ClientLang"].ToString().ToLower();
        }

        public async Task<AdministrationGetPostersResponse> Handle(AdministrationGetPostersRequest request, CancellationToken cancellationToken = default)
        {
            var userId = _loginService.GetUserId();
            if (userId == Guid.Empty)
                return null;

            var posters = await _repository.GetListNoTracking(_lang, userId, request.CategoryId, request.Status, request.StartDate, request.EndDate, null, request.CityId);
            var cities = await _repository.GetCities();
            var categories = await _repository.GetCategories();
            var subcategories = await _repository.GetSubcategories();
            var result = new AdministrationGetPostersResponse()
            {
                Count = posters.Count,
                CountPerPage = request.CountPerPage,
                Page = request.Page + 1
            };

            if (request.Sort == 0)
            {
                posters = posters.OrderByDescending(x => x.CreatedAt).ToList();
            }
            else if (request.Sort == 1)
            {
                posters = posters.OrderByDescending(x => x.UpdatedAt).ToList();
            }
            else
            {
                posters = posters.OrderBy(x => x.Status).ToList();
            }

            posters = posters.Skip(request.Page * request.CountPerPage).Take(request.CountPerPage).ToList();
            var idEnum = posters.Select(x => x.Id);
            var multilang = await _repository.GetMultilang(idEnum);
            var fullInfo = await _repository.GetFullInfo(idEnum);

            var posterList = posters.Select(o => new AdministrationPostersResponse()
            {
                Id = o.Id,
                CategoryId = o.CategoryId,
                CategoryName = categories.Where(c => c.Id == o.CategoryId).Select(c => c.Name).FirstOrDefault(),
                Name = multilang.Where(m => m.PosterId == o.Id).Select(m => m.Name).FirstOrDefault(),
                CityId = fullInfo.Where(f => f.OrganizationId == o.Id).Select(f => f.City).FirstOrDefault(),
                CityName = fullInfo.Where(f => f.OrganizationId == o.Id)
                                   .Join(cities,
                                         f => f.City,
                                         c => c.Id,
                                         (f, c) => c.Name)
                                   .FirstOrDefault(),
                CreatedAt = o.CreatedAt,
                Status = o.Status,
                UpdatedAt = o.UpdatedAt,
                ReleaseDate = o.ReleaseDate
            }).ToList();

            if (request.Sort == 0)
            {
                posterList = posterList.OrderByDescending(x => x.CreatedAt).ToList();
            }
            else if (request.Sort == 1)
            {
                posterList = posterList.OrderByDescending(x => x.UpdatedAt).ToList();
            }
            else
            {
                posterList = posterList.OrderBy(x => x.Status).ToList();
            }

            posterList = posterList.Skip(request.Page * request.CountPerPage).Take(request.CountPerPage).ToList();
            result.Posters = posterList;

            return result;
        }
    }
}
