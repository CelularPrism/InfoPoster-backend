using InfoPoster_backend.Models.Administration;
using InfoPoster_backend.Models.Banner;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Selectel_API;
using InfoPoster_backend.Tools;
using MediatR;

namespace InfoPoster_backend.Handlers.Administration.Banner
{
    public class GetPopularityBannerRequest : IRequest<List<BannerResponseModel>>
    {
        public POPULARITY_PLACE Place { get; set; }
        public Guid CityId { get; set; }
    }

    public class GetPopularityBannerHandler : IRequestHandler<GetPopularityBannerRequest, List<BannerResponseModel>>
    {
        private readonly BannerRepository _repository;
        private readonly FileRepository _file;
        private readonly SelectelAuthService _selectelAuthService;

        public GetPopularityBannerHandler(BannerRepository repository, FileRepository file, SelectelAuthService selectelAuthService)
        {
            _repository = repository;
            _file = file;
            _selectelAuthService = selectelAuthService;
        }

        public async Task<List<BannerResponseModel>> Handle(GetPopularityBannerRequest request, CancellationToken cancellationToken = default)
        {
            var popular = await _repository.GetPopularBannerList(POPULARITY_PLACE.MAIN, request.CityId);
            var loggedIn = await _selectelAuthService.Login();
            var result = new List<BannerResponseModel>();

            if (loggedIn)
            {
                var selectelUUID = await _selectelAuthService.GetContainerUUID("dosdoc");
                foreach (var item in popular)
                {
                    var id = item.Id;
                    if (item.ApplicationId != null && item.Type != null)
                        id = (Guid)item.ApplicationId;

                    var fileList = await _file.GetSelectelFiles(id, 0);
                    var file = fileList.FirstOrDefault();
                    var url = string.Empty;

                    if (file != null)
                        url = string.Concat("https://", selectelUUID, ".selstorage.ru/", id);

                    result.Add(new BannerResponseModel()
                    {
                        Comment = item.Comment,
                        ExternalLink = item.ExternalLink,
                        FileURL = url,
                        Id = item.Id,
                        ReleaseDate = item.ReleaseDate,
                        UserId = item.UserId
                    });
                }
            }
            return result;
        }
    }
}
