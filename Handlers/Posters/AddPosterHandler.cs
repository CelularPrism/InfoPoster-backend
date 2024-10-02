using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Login;
using MediatR;

namespace InfoPoster_backend.Handlers.Posters
{
    public class AddPosterRequest : IRequest<AddPosterResponse> { }

    public class AddPosterResponse
    {
        public Guid Id { get; set; }
    }

    public class AddPosterHandler : IRequestHandler<AddPosterRequest, AddPosterResponse> 
    {
        private readonly LoginService _loginService;
        private readonly PosterRepository _repository;
        private readonly string _lang;
        public AddPosterHandler(LoginService loginService, PosterRepository repository, IHttpContextAccessor accessor)
        {
            _loginService = loginService;
            _repository = repository;
            _lang = accessor.HttpContext.Items["ClientLang"].ToString().ToLower();
        }

        public async Task<AddPosterResponse> Handle(AddPosterRequest request, CancellationToken cancellationToken = default)
        {
            var user = _loginService.GetUserId();
            if (user == Guid.Empty)
                return null;

            var poster = new PosterModel()
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow,
                Status = (int)POSTER_STATUS.DISABLED,
                UserId = user
            };

            var multilang = new PosterMultilangModel()
            {
                Id = Guid.NewGuid(),
                Lang = _lang,
                PosterId = poster.Id
            };

            await _repository.AddPosterMultilang(multilang);
            await _repository.AddPoster(poster);
            var result = new AddPosterResponse()
            {
                Id = poster.Id
            };

            return result;
        }
    }
}
