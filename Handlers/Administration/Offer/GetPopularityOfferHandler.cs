using InfoPoster_backend.Handlers.Administration.Poster;
using InfoPoster_backend.Models.Administration;
using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Administration.Offer
{
    public class GetPopularityOfferRequest : IRequest<List<GetPopularityOfferResponse>>
    {
        public POPULARITY_PLACE Place { get; set; }

    }

    public class GetPopularityOfferResponse
    {
        public Guid? Id { get; set; }
        public Guid OfferId { get; set; }
        public string Name { get; set; }
        public int? Popularity { get; set; }
    }

    public class GetPopularityOfferHandler : IRequestHandler<GetPopularityOfferRequest, List<GetPopularityOfferResponse>>
    {
        private readonly OfferRepository _repository;
        public GetPopularityOfferHandler(OfferRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<GetPopularityOfferResponse>> Handle(GetPopularityOfferRequest request, CancellationToken cancellation = default)
        {
            var posters = await _repository.GetPopularOfferList(request.Place);
            var popularity = await _repository.GetPopularityList(request.Place);

            var result = posters.Select(o => new GetPopularityOfferResponse()
            {
                Id = popularity.Any(p => p.ApplicationId == o.Id) ? popularity.Where(p => p.ApplicationId == o.Id).Select(p => p.Id).FirstOrDefault() : null,
                OfferId = o.Id,
                Name = o.Name,
                Popularity = popularity.Any(p => p.ApplicationId == o.Id) ? popularity.Where(p => p.ApplicationId == o.Id).Select(p => p.Popularity).FirstOrDefault() : null
            }).ToList();
            return result;
        }
    }
}
