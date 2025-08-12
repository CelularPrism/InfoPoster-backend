using InfoPoster_backend.Models.Administration;
using InfoPoster_backend.Repos;
using MediatR;
using System.Runtime.CompilerServices;

namespace InfoPoster_backend.Handlers.Administration.Banner
{
    public class RemovePopularityRequest : IRequest<bool>
    {
        public POPULARITY_PLACE PopularityPlace { get; set; }
    }

    public class RemovePopularityHandler : IRequestHandler<RemovePopularityRequest, bool>
    {
        private readonly BannerRepository _repository;
        public RemovePopularityHandler(BannerRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(RemovePopularityRequest request, CancellationToken cancellationToken = default)
        {
            var oldPopularity = await _repository.GetPopularity(request.PopularityPlace);
            await _repository.RemoveRangePopularity(oldPopularity);
            return true;
        }
    }
}
