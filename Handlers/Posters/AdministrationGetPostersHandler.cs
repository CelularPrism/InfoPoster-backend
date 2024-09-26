using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Posters
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
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime ReleaseDate { get; set; }
        public Guid CategoryId { get; set; }
        public string CreatedBy { get; set; }
    }

    public class AdministrationGetPostersHandler : IRequestHandler<AdministrationGetPostersRequest, List<AdministrationGetPostersResponse>>
    {
        private readonly PosterRepository _repository;
        private readonly string _lang;

        public AdministrationGetPostersHandler(PosterRepository repository, IHttpContextAccessor accessor)
        {
            _repository = repository;
            _lang = accessor.HttpContext.Items["ClientLang"].ToString().ToLower();
        }

        public async Task<List<AdministrationGetPostersResponse>> Handle(AdministrationGetPostersRequest request, CancellationToken cancellationToken = default)
        {
            var posterList = await _repository.GetListNoTracking(_lang);
            return posterList;
        }
    }
}
