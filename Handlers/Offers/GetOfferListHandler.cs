using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Offers;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Selectel_API;
using MediatR;

namespace InfoPoster_backend.Handlers.Offers
{
    public class GetOfferListRequest : IRequest<List<GetOfferListResponse>> { }

    public class GetOfferListResponse
    {
        public Guid Id { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public string Name { get; set; }
        public Guid? FileId { get; set; }
        public string FileURL { get; set; }
    }

    public class GetOfferListHandler : IRequestHandler<GetOfferListRequest, List<GetOfferListResponse>>
    {
        private readonly OfferRepository _repository;
        private readonly FileRepository _file;
        private readonly SelectelAuthService _selectelAuth;

        public GetOfferListHandler(OfferRepository repository, FileRepository file, SelectelAuthService selectelAuthService)
        {
            _repository = repository;
            _file = file;
            _selectelAuth = selectelAuthService;
        }

        public async Task<List<GetOfferListResponse>> Handle(GetOfferListRequest request, CancellationToken cancellationToken = default)
        {
            var offers = await _repository.GetOffersByCity();

            var result = offers.Select(o => new GetOfferListResponse()
            {
                Id = o.Id,
                DateEnd = o.DateEnd,
                DateStart = o.DateStart,
                Name = o.Name
            }).ToList();
            var loggedIn = await _selectelAuth.Login();
            if (loggedIn)
            {
                var selectelUUID = await _selectelAuth.GetContainerUUID("dosdoc");
                foreach (var item in result)
                {
                    var file = await _file.GetPrimaryFile(item.Id, (int)FILE_PLACES.GALLERY);
                    if (file == null)
                        file = await _file.GetApplicationFileByApplication(item.Id);

                    if (file != null)
                    {
                        item.FileId = file.FileId;
                        item.FileURL = string.Concat("https://", selectelUUID, ".selstorage.ru/", item.FileId);
                    }
                }
            }

            result = result.Take(6).ToList();
            return result;
        }
    }
}
