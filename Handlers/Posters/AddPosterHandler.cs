using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Login;
using InfoPoster_backend.Tools;
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
        public AddPosterHandler(LoginService loginService, PosterRepository repository)
        {
            _loginService = loginService;
            _repository = repository;
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

            
            var multilang = new List<PosterMultilangModel>();

            foreach (var lang in Constants.SystemLangs)
            {
                multilang.Add(new PosterMultilangModel()
                {
                    Id = Guid.NewGuid(),
                    Lang = lang,
                    PosterId = poster.Id
                });
            }

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
