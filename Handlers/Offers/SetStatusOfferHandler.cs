using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Offers
{
    public class SetStatusOfferRequest : IRequest<SetStatusOfferResponse>
    {
        public Guid Id { get; set; }
        public POSTER_STATUS Status { get; set; }
    }

    public class SetStatusOfferResponse { }

    public class SetStatusOfferHandler : IRequestHandler<SetStatusOfferRequest, SetStatusOfferResponse>
    {
        private readonly OfferRepository _repository;

        public SetStatusOfferHandler(OfferRepository repository)
        {
            _repository = repository;
        }

        public async Task<SetStatusOfferResponse> Handle(SetStatusOfferRequest request, CancellationToken cancellation)
        {
            var offer = await _repository.GetOffer(request.Id);
            if (offer == null)
            {
                return null;
            }

            var multilang = await _repository.GetOfferMultilang(request.Id);
            foreach (var ml in multilang)
            {
                if (string.IsNullOrEmpty(ml.Name) || string.IsNullOrEmpty(ml.DateDescription) || string.IsNullOrEmpty(ml.SmallDescription) || string.IsNullOrEmpty(ml.Description))
                {
                    return null;
                }
            }

            if (offer.DateStart == DateTime.MinValue || offer.DateEnd == DateTime.MinValue || !multilang.Any())
            {
                return null;
            }

            offer.Status = request.Status;
            await _repository.UpdateOffer(offer);
            return new SetStatusOfferResponse();
        }
    }
}
