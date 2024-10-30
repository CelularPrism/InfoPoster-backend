using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Login;
using InfoPoster_backend.Tools;
using MediatR;
using System.Text.Json.Serialization;

namespace InfoPoster_backend.Handlers.Administration
{
    public class AdministrationGetPostersRequest : IRequest<List<AdministrationGetPostersResponse>> { }

    public class AdministrationGetPostersResponse
    {
        public AdministrationGetPostersResponse(PosterModel poster, PosterMultilangModel multilang, string userName)
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

    public class AdministrationGetPostersHandler : IRequestHandler<AdministrationGetPostersRequest, List<AdministrationGetPostersResponse>>
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

        public async Task<List<AdministrationGetPostersResponse>> Handle(AdministrationGetPostersRequest request, CancellationToken cancellationToken = default)
        {
            var userId = _loginService.GetUserId();
            if (userId == Guid.Empty)
                return null;

            var posterList = await _repository.GetListNoTracking(userId, _lang);
            return posterList;
        }
    }
}
