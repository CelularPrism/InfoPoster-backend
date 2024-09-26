using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
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
        public ChangePosterStatusHandler(PosterRepository repository)
        {
            _repository = repository;
        }

        public async Task<ChangePosterStatusResponse> Handle(ChangePosterStatusRequest request, CancellationToken cancellationToken = default)
        {
            var poster = await _repository.GetPoster(request.Id);
            if (poster == null)
                return null;

            poster.Status = (int)request.Status;
            await _repository.UpdatePoster(poster);

            return new ChangePosterStatusResponse();
        }
    }
}
