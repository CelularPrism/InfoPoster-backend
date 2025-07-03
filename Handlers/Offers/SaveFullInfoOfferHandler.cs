using InfoPoster_backend.Models.Offers;
using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Login;
using InfoPoster_backend.Tools;
using MediatR;

namespace InfoPoster_backend.Handlers.Offers
{
    public class SaveFullInfoOfferRequest : IRequest<SaveFullInfoOfferResponse>
    {
        public Guid Id { get; set; }
        public Guid CityId { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public OFFER_TYPES? Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Lang { get; set; }
    }

    public class SaveFullInfoOfferResponse { }

    public class SaveFullInfoOfferHandler : IRequestHandler<SaveFullInfoOfferRequest, SaveFullInfoOfferResponse>
    {
        private readonly OfferRepository _repository;
        private readonly Guid _user;

        public SaveFullInfoOfferHandler(OfferRepository repository, LoginService loginService)
        {
            _repository = repository;
            _user = loginService.GetUserId();
        }

        public async Task<SaveFullInfoOfferResponse> Handle(SaveFullInfoOfferRequest request, CancellationToken cancellationToken = default)
        {
            var offer = await _repository.GetOffer(request.Id);
            if (offer == null || offer.Status == POSTER_STATUS.PUBLISHED)
            {
                return null;
            }

            //var multilang = await _repository.GetOfferMultilang(request.Id, request.Lang);
            //if (multilang == null)
            //{
            //    multilang = new OffersMultilangModel(request.Id, request.Lang, request.Name, request.Description, request.Address);
            //    await _repository.AddOfferMultilang(multilang);
            //} else
            //{
            //    multilang.Update(request.Lang, request.Name, request.Description, request.Address);
            //    await _repository.UpdateOfferMultilang(multilang);
            //}

            var multilang = await _repository.GetOfferMultilang(request.Id);
            if (multilang == null || multilang.Count == 0)
            {
                multilang = new List<OffersMultilangModel>();
                OffersMultilangModel ml;
                foreach (var lang in Constants.SystemLangs)
                {
                    ml = new OffersMultilangModel(request.Id, request.Lang, request.Name, request.Description);
                    multilang.Add(ml);
                }

                await _repository.AddOfferMultilang(multilang);
            }
            else
            {
                foreach (var ml in multilang)
                {
                    ml.Update(request.Lang, request.Name, request.Description);
                }
                await _repository.UpdateOfferMultilang(multilang);
            }

            if (string.IsNullOrEmpty(offer.Name) || request.Lang == Constants.DefaultLang)
                offer.Name = request.Name;

            if (request.Type != null)
                offer.Type = request.Type.Value;

            offer.DateStart = request.DateStart;
            offer.DateEnd = request.DateEnd;
            //offer.PlaceLink = request.PlaceLink;
            offer.CityId = request.CityId;
            await _repository.UpdateOffer(offer);

            return new SaveFullInfoOfferResponse();
        }
    }
}
