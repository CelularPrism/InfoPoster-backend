using InfoPoster_backend.Models.Administration;
using InfoPoster_backend.Models.Banner;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Selectel_API;
using InfoPoster_backend.Tools;
using MediatR;

namespace InfoPoster_backend.Handlers.Administration.Banner
{
    public class GetPopularityBannerRequest : IRequest<List<GetPopularityBannerResponse>>
    {
        public POPULARITY_PLACE Place { get; set; }
        public Guid CityId { get; set; }
        public Guid? PlaceId { get; set; }
    }

    public class GetPopularityBannerResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string ExternalLink { get; set; }
        public string Comment { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string FileURL { get; set; }
        public Guid? FileId { get; set; }
        public int? Popularity { get; set; }
    }

    public class GetPopularityBannerHandler : IRequestHandler<GetPopularityBannerRequest, List<GetPopularityBannerResponse>>
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

        public async Task<List<GetPopularityBannerResponse>> Handle(GetPopularityBannerRequest request, CancellationToken cancellationToken = default)
        {
            var popular = await _repository.GetPopularBannerList(request.Place, request.CityId, request.PlaceId);
            var loggedIn = await _selectelAuthService.Login();
            var result = new List<GetPopularityBannerResponse>();

            if (loggedIn)
            {
                var selectelUUID = await _selectelAuthService.GetContainerUUID("dosdoc");
                foreach (var item in popular)
                {
                    var id = item.Id;

                    var fileList = await _file.GetSelectelFiles(id, 0);
                    var file = fileList.FirstOrDefault();
                    var url = string.Empty;

                    if (file != null)
                        url = string.Concat("https://", selectelUUID, ".selstorage.ru/", file.Id);

                    result.Add(new GetPopularityBannerResponse()
                    {
                        ExternalLink = item.ExternalLink,
                        FileURL = url,
                        FileId = file != null ? file.Id : null,
                        Id = item.Id,
                        ReleaseDate = item.ReleaseDate,
                        UserId = item.UserId,
                        Popularity = item.Popularity,
                        Comment = item.Comment,
                    });
                }
            }
            return result;
        }
    }
}
