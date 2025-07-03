using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Offers;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Selectel_API;
using MediatR;

namespace InfoPoster_backend.Handlers.Offers
{
    public class GetOfferFullInfoRequest : IRequest<OffersResponseModel>
    {
        public Guid Id { get; set; }
    }

    public class GetOfferFullInfoHandler : IRequestHandler<GetOfferFullInfoRequest, OffersResponseModel>
    {
        private readonly OfferRepository _repository;
        private readonly FileRepository _file;
        private readonly SelectelAuthService _selectelAuth;

        public GetOfferFullInfoHandler(OfferRepository repository, FileRepository file, SelectelAuthService selectelAuth)
        {
            _repository = repository;
            _file = file;
            _selectelAuth = selectelAuth;
        }

        public async Task<OffersResponseModel> Handle(GetOfferFullInfoRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _repository.GetOfferResponse(request.Id);

            if (result == null)
                return null;

            var loggedIn = await _selectelAuth.Login();
            if (loggedIn)
            {
                var selectelUUID = await _selectelAuth.GetContainerUUID("dosdoc");
                var file = await _file.GetPrimaryFile(result.Id, (int)FILE_PLACES.GALLERY);
                if (file == null)
                    file = await _file.GetApplicationFileByApplication(result.Id);

                if (file != null)
                {
                    result.FileId = file.FileId;
                    result.FileURL = string.Concat("https://", selectelUUID, ".selstorage.ru/", result.FileId);
                }
            }

            return result;
        }
    }
}
