using InfoPoster_backend.Models.Offers;
using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Administration.Offer
{
    public class GetPublishedOfferRequest : IRequest<List<OffersModel>>
    {
        public string SearchText { get; set; }
        public Guid CityId { get; set; }
    }

    public class GetPublishedOfferHandler : IRequestHandler<GetPublishedOfferRequest, List<OffersModel>>
    {
        private readonly OfferRepository _repository;

        public GetPublishedOfferHandler(OfferRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<OffersModel>> Handle(GetPublishedOfferRequest request, CancellationToken cancellation = default)
        {
            var offers = await _repository.GetOfferList(request.CityId);

            var result = offers.Where(o => o.Name.Contains(request.SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
            return result;
        }
    }
}
