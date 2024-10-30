using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Login;
using InfoPoster_backend.Tools;
using MediatR;
using System.Text.Json.Serialization;

namespace InfoPoster_backend.Handlers.Administration
{
    public class GetAllPostersRequest : IRequest<List<GetAllPostersResponse>> { } 

    public class GetAllPostersResponse
    {
        public GetAllPostersResponse(PosterModel poster, PosterMultilangModel multilang, string userName)
        {
            Id = poster.Id;
            Name = multilang.Name;
            ReleaseDate = poster.ReleaseDate;
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
    }

    public class GetAllPostersHandler : IRequestHandler<GetAllPostersRequest, List<GetAllPostersResponse>>
    {
        private readonly PosterRepository _repository;
        private readonly string _lang;

        public GetAllPostersHandler(PosterRepository repository, IHttpContextAccessor accessor)
        {
            _repository = repository;
            _lang = accessor.HttpContext.Items["ClientLang"].ToString().ToLower();
        }

        public async Task<List<GetAllPostersResponse>> Handle(GetAllPostersRequest request, CancellationToken cancellation = default)
        {
            var posters = await _repository.GetListNoTracking(_lang);
            return posters;
        }
    }
}
