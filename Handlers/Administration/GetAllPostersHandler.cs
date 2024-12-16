using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Login;
using InfoPoster_backend.Tools;
using MediatR;
using System.Text.Json.Serialization;

namespace InfoPoster_backend.Handlers.Administration
{
    public class GetAllPostersRequest : IRequest<GetAllPostersResponse>
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

    public class GetAllPostersResponse
    {
        public List<AllPostersResponse> Posters { get; set; }
        public int Count { get; set; }
        public int Page { get; set; }
        public int CountPerPage { get; set; }
    }
    public class AllPostersResponse
    {
        public AllPostersResponse() { }

        public AllPostersResponse(PosterModel poster, PosterMultilangModel multilang, string userName)
        {
            Id = poster.Id;
            Name = multilang.Name;
            ReleaseDate = poster.ReleaseDate;
            CategoryId = poster.CategoryId;
            UserId = poster.UserId;
            CreatedBy = userName;
            Status = poster.Status;
            CreatedAt = poster.CreatedAt;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        [JsonConverter(typeof(OnlyDateConverter))]
        public DateTime? ReleaseDate { get; set; }
        public Guid CategoryId { get; set; }
        public Guid UserId { get; set; }
        public string CreatedBy { get; set; }
        public int Status { get; set; }
        public string CategoryName { get; set; }
        public Guid? CityId { get; set; }
        public string CityName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class GetAllPostersHandler : IRequestHandler<GetAllPostersRequest, GetAllPostersResponse>
    {
        private readonly PosterRepository _repository;
        private readonly string _lang;
        private readonly Guid _user;

        public GetAllPostersHandler(PosterRepository repository, IHttpContextAccessor accessor, LoginService loginService)
        {
            _repository = repository;
            _lang = accessor.HttpContext.Items["ClientLang"].ToString().ToLower();
            _user = loginService.GetUserId();
        }

        public async Task<GetAllPostersResponse> Handle(GetAllPostersRequest request, CancellationToken cancellation = default)
        {
            var posters = await _repository.GetListNoTracking(_lang, _user, request.CategoryId, request.Status, request.StartDate, request.EndDate, request.UserId, request.CityId);
            var cities = await _repository.GetCities();
            var categories = await _repository.GetCategories();
            var subcategories = await _repository.GetSubcategories();
            var result = new GetAllPostersResponse()
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
            var userEnum = posters.Select(x => x.UserId);
            var multilang = await _repository.GetMultilang(idEnum);
            var fullInfo = await _repository.GetFullInfo(idEnum);
            var users = await _repository.GetUsers(userEnum);

            var orgList = posters.Select(o => new AllPostersResponse()
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
                CreatedBy = users.Where(u => u.Id == o.UserId).Select(u => u.FirstName + " " + u.LastName).FirstOrDefault(),
                UserId = o.UserId,
                Status = o.Status,
                UpdatedAt = o.UpdatedAt,
                ReleaseDate = o.ReleaseDate
            }).ToList();

            if (request.Sort == 0)
            {
                orgList = orgList.OrderByDescending(x => x.CreatedAt).ToList();
            } else if (request.Sort == 1) 
            {
                orgList = orgList.OrderByDescending(x => x.UpdatedAt).ToList();
            } else
            {
                orgList = orgList.OrderBy(x => x.Status).ToList();
            }
            result.Posters = orgList;

            return result;
        }
    }
}
