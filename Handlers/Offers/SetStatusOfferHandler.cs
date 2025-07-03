using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Login;
using MediatR;

namespace InfoPoster_backend.Handlers.Offers
{
    public class SetStatusOfferRequest : IRequest<SetStatusOfferResponse>
    {
        public Guid Id { get; set; }
        public POSTER_STATUS Status { get; set; }
        public string Comment { get; set; }
    }

    public class SetStatusOfferResponse { }

    public class SetStatusOfferHandler : IRequestHandler<SetStatusOfferRequest, SetStatusOfferResponse>
    {
        private readonly OfferRepository _repository;
        private readonly Guid _user;

        public SetStatusOfferHandler(OfferRepository repository, LoginService loginService)
        {
            _repository = repository;
            _user = loginService.GetUserId();
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
                if (string.IsNullOrEmpty(ml.Name) || string.IsNullOrEmpty(ml.Description))
                {
                    return null;
                }
            }

            if (offer.DateStart == DateTime.MinValue || !multilang.Any())
            {
                return null;
            }

            if (request.Status == POSTER_STATUS.REJECTED && !string.IsNullOrEmpty(request.Comment))
                await _repository.AddRejectedComment(new RejectedComments() { ApplicationId = offer.Id, UserId = _user, Text = request.Comment });

            offer.Status = request.Status;
            await _repository.UpdateOffer(offer);
            return new SetStatusOfferResponse();
        }
    }
}
