using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Posters
{
    public class GetFullInfoPosterRequest : IRequest<PosterFullInfoResponseModel>
    {
        public Guid Id { get; set; }
    }

    public class GetFullInfoPosterHandler : IRequestHandler<GetFullInfoPosterRequest, PosterFullInfoResponseModel>
    {
        private readonly PosterRepository _repository;
        public GetFullInfoPosterHandler(PosterRepository repository)
        {
            _repository = repository;
        }

        public async Task<PosterFullInfoResponseModel> Handle(GetFullInfoPosterRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _repository.GetFullInfo(request.Id);
            return result;
        }
    }
}
