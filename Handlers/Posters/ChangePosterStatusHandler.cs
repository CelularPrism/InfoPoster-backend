using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Login;
using MediatR;

namespace InfoPoster_backend.Handlers.Posters
{
    public class ChangePosterStatusRequest : IRequest<ChangePosterStatusResponse>
    {
        public Guid Id { get; set; }
        public POSTER_STATUS Status { get; set; }
    }

    public class ChangePosterStatusResponse { }

    public class ChangePosterStatusHandler : IRequestHandler<ChangePosterStatusRequest, ChangePosterStatusResponse>
    {
        private readonly PosterRepository _repository;
        private readonly Guid _user;

        public ChangePosterStatusHandler(PosterRepository repository, LoginService loginService)
        {
            _repository = repository;
            _user = loginService.GetUserId();
        }

        public async Task<ChangePosterStatusResponse> Handle(ChangePosterStatusRequest request, CancellationToken cancellationToken = default)
        {
            var poster = await _repository.GetPoster(request.Id);
            if (poster == null)
                return null;

            poster.Status = (int)request.Status;
            await _repository.UpdatePoster(poster, _user);

            return new ChangePosterStatusResponse();
        }
    }
}
