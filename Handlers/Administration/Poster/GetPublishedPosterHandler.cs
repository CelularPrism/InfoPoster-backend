using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Administration.Poster
{
    public class GetPublishedPosterRequest : IRequest<List<PosterModel>>
    {
        public string SearchText { get; set; }
    }

    public class GetPublishedPosterHandler : IRequestHandler<GetPublishedPosterRequest, List<PosterModel>>
    {
        private readonly PosterRepository _repository;

        public GetPublishedPosterHandler(PosterRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<PosterModel>> Handle(GetPublishedPosterRequest request, CancellationToken cancellation = default)
        {
            var posters = await _repository.GetPosterList();

            var result = posters.Where(o => o.Name.Contains(request.SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
            return result;
        }
    }
}
