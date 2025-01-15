using InfoPoster_backend.Models.Offers;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Login;
using MediatR;

namespace InfoPoster_backend.Handlers.Offers
{
    public class CreateOfferRequest : IRequest<CreateOfferResponse>
    {
        public OFFER_TYPES type { get; set; }
    }

    public class CreateOfferResponse
    {
        public Guid Id { get; set; }
    }

    public class CreateOfferHandler : IRequestHandler<CreateOfferRequest, CreateOfferResponse>
    {
        private readonly OfferRepository _repository;
        private readonly Guid _user;

        public CreateOfferHandler(OfferRepository repository, LoginService loginService)
        {
            _repository = repository;
            _user = loginService.GetUserId();
        }

        public async Task<CreateOfferResponse> Handle(CreateOfferRequest request, CancellationToken cancellationToken = default)
        {
            var offer = new OffersModel();
            offer.Type = request.type;
            offer.Status = Models.Posters.POSTER_STATUS.DRAFT;
            offer.UserId = _user;
            await _repository.AddOffer(offer);
            return new CreateOfferResponse() { Id = offer.Id };
        }
    }
}
