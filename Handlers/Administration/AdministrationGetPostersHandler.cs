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

            var posterList = await _repository.GetListNoTracking(userId, _lang);

            if (request.CategoryId != null)
            {
                posterList = posterList.Where(x => x.CategoryId == request.CategoryId).ToList();
            }

            if (request.CityId != null)
            {
                posterList = posterList.Where(x => x.CityId == request.CityId).ToList();
            }

            if (request.Status != null)
            {
                posterList = posterList.Where(x => x.Status == request.Status).ToList();
            }

            if (request.StartDate != null)
            {
                posterList = posterList.Where(x => x.CreatedAt >= request.StartDate).ToList();
            }

            if (request.EndDate != null)
            {
                posterList = posterList.Where(x => x.CreatedAt <= request.EndDate).ToList();
            }

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

            var result = new AdministrationGetPostersResponse()
            {
                Count = posterList.Count,
                CountPerPage = request.CountPerPage,
                Page = request.Page,
            };
            posterList = posterList.Skip(request.Page * request.CountPerPage).Take(request.CountPerPage).ToList();
            result.Posters = posterList;

            return result;
        }
    }
}
