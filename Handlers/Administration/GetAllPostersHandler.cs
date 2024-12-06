using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
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
        public Guid? LastUpdatedBy { get; set; }
        public string LastUpdatedByName { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
    }

    public class GetAllPostersHandler : IRequestHandler<GetAllPostersRequest, GetAllPostersResponse>
    {
        private readonly PosterRepository _repository;
        private readonly string _lang;

        public GetAllPostersHandler(PosterRepository repository, IHttpContextAccessor accessor)
        {
            _repository = repository;
            _lang = accessor.HttpContext.Items["ClientLang"].ToString().ToLower();
        }

        public async Task<GetAllPostersResponse> Handle(GetAllPostersRequest request, CancellationToken cancellation = default)
        {
            var posters = await _repository.GetListNoTracking(_lang);

            if (request.CategoryId != null)
            {
                posters = posters.Where(x => x.CategoryId == request.CategoryId).ToList();
            }

            if (request.CityId != null)
            {
                posters = posters.Where(x => x.CityId == request.CityId).ToList();
            }

            if (request.Status != null)
            {
                posters = posters.Where(x => x.Status == request.Status).ToList();
            }

            if (request.StartDate != null)
            {
                posters = posters.Where(x => x.CreatedAt >= request.StartDate).ToList();
            }

            if (request.EndDate != null)
            {
                posters = posters.Where(x => x.CreatedAt <= request.EndDate).ToList();
            }

            if (request.UserId != null)
            {
                posters = posters.Where(x => x.UserId == request.UserId).ToList();
            }

            if (request.Sort == 0)
            {
                posters = posters.OrderByDescending(x => x.CreatedAt).ToList();
            } else if (request.Sort == 1) 
            {
                posters = posters.OrderByDescending(x => x.UpdatedAt).ToList();
            } else
            {
                posters = posters.OrderBy(x => x.Status).ToList();
            }

            var result = new GetAllPostersResponse()
            {
                Count = posters.Count,
                CountPerPage = request.CountPerPage,
                Page = request.Page + 1,
            };
            posters = posters.Skip(request.Page * request.CountPerPage).Take(request.CountPerPage).ToList();
            result.Posters = posters;

            return result;
        }
    }
}
