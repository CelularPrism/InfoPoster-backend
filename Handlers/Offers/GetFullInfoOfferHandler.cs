using InfoPoster_backend.Models.Offers;
using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Offers
{
    public class GetFullInfoOfferRequest : IRequest<GetFullInfoOfferResponse>
    {
        public Guid Id { get; set; }
        public string Lang { get; set; }
    }

    public class GetFullInfoOfferResponse
    {
        public Guid Id { get; set; }
        public Guid CityId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public Guid UserId { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }
        public string Lang { get; set; }
        public string Name { get; set; }
        public string DateDescription { get; set; }
        public string SmallDescription { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
    }

    public class GetFullInfoOfferHandler : IRequestHandler<GetFullInfoOfferRequest, GetFullInfoOfferResponse>
    {
        private readonly OfferRepository _repository;
        public GetFullInfoOfferHandler(OfferRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetFullInfoOfferResponse> Handle(GetFullInfoOfferRequest request, CancellationToken cancellationToken = default)
        {
            var offer = await _repository.GetOffer(request.Id);
            if (offer == null)
            {
                return null;
            }

            var ml = await _repository.GetOfferMultilang(request.Id, request.Lang);
            if (ml == null)
            {
                ml = new OffersMultilangModel();
            }

            var result = new GetFullInfoOfferResponse()
            {
                Id = offer.Id,
                Lang = ml.Lang,
                Name = ml.Name,
                DateDescription = ml.DateDescription,
                SmallDescription = ml.SmallDescription,
                Address = ml.Address,
                CityId = offer.CityId,
                CreatedAt = offer.CreatedAt,
                DateEnd = offer.DateEnd,
                DateStart = offer.DateStart,
                Description = ml.Description,
                Status = (int)offer.Status,
                Type = (int)offer.Type,
                UserId = offer.UserId,
            };
            return result;
        }
    }
}
