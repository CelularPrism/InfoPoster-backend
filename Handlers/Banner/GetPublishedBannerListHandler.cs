using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Administration;
using InfoPoster_backend.Models.Banner;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Selectel_API;
using InfoPoster_backend.Tools;
using MediatR;

namespace InfoPoster_backend.Handlers.Banner
{
    public class GetPublishedBannerListRequest : IRequest<List<BannerResponseModel>>
    {
        public POPULARITY_PLACE Place { get; set; }
        public Guid? PlaceId { get; set; }
    }

    public class GetPublishedBannerListHandler : IRequestHandler<GetPublishedBannerListRequest, List<BannerResponseModel>>
    {
        private readonly BannerRepository _repository;
        private readonly PosterRepository _poster;
        private readonly OrganizationRepository _organization;
        private readonly FileRepository _file;
        private readonly SelectelAuthService _selectelAuth;
        private readonly Guid _city;

        public GetPublishedBannerListHandler(BannerRepository repository, PosterRepository poster, OrganizationRepository organization, FileRepository file, SelectelAuthService selectelAuth, IHttpContextAccessor accessor)
        {
            _repository = repository;
            _poster = poster;
            _organization = organization;
            _file = file;
            _selectelAuth = selectelAuth;
            _city = Guid.TryParse(accessor.HttpContext.Request.Headers["X-Testing"].ToString(), out _city) ? Guid.Parse(accessor.HttpContext.Request.Headers["X-Testing"].ToString()) : Constants.DefaultCity;
        }

        public async Task<List<BannerResponseModel>> Handle(GetPublishedBannerListRequest request, CancellationToken cancellationToken = default)
        {
            var bannerList = await _repository.GetPopularBannerList(request.Place, _city, request.PlaceId);
            var result = new List<BannerResponseModel>();

            if (request.PlaceId != null)
            {
                bannerList = bannerList.Where(b => b.PlaceId == request.PlaceId).ToList();
            }

            if (bannerList.Count < 9)
            {
                if (request.Place == POPULARITY_PLACE.CATEGORY_PLACE || request.Place == POPULARITY_PLACE.SUBCATEGORY_PLACE)
                {
                    var posters = await _poster.GetPopularPosterList(request.Place, _city, request.PlaceId);
                    var postersBanner = posters.Select(p => new BannerResponseModel()
                    {
                        Id = Guid.NewGuid(),
                        ExternalLink = string.Concat("application/event/" + p.Id),
                        ReleaseDate = DateTime.UtcNow,
                        UserId = Guid.NewGuid(),
                        ApplicationId = p.Id
                    }).ToList();
                    result.AddRange(postersBanner);
                } else if (request.Place == POPULARITY_PLACE.CATEGORY_EVENT || request.Place == POPULARITY_PLACE.SUBCATEGORY_EVENT)
                {
                    var organization = await _organization.GetPopularOrganizationList(request.Place, _city, request.PlaceId);
                    var organizationBanner = organization.Select(o => new BannerResponseModel()
                    {
                        Id = Guid.NewGuid(),
                        ExternalLink = string.Concat("application/place/" + o.Id),
                        ReleaseDate = DateTime.UtcNow,
                        UserId = Guid.NewGuid(),
                        ApplicationId = o.Id
                    }).ToList();
                    result.AddRange(organizationBanner);
                }
            }

            if (bannerList.Count > 9)
                bannerList = bannerList.Take(9).ToList();

            result = bannerList.Select(b => new BannerResponseModel()
            {
                ExternalLink = b.ExternalLink,
                Id = b.Id,
                ReleaseDate = b.ReleaseDate,
                UserId = b.UserId,
            }).ToList();

            var loggedIn = await _selectelAuth.Login();
            if (loggedIn)
            {
                var selectelUUID = await _selectelAuth.GetContainerUUID("dosdoc");
                foreach (var item in result)
                {
                    var id = item.Id;
                    if (item.ApplicationId != null)
                        id = (Guid)item.ApplicationId;

                    var file = await _file.GetPrimaryFile(id, (int)FILE_PLACES.GALLERY);
                    if (file == null)
                        file = await _file.GetApplicationFileByApplication(id);

                    if (file != null)
                    {
                        item.FileURL = string.Concat("https://", selectelUUID, ".selstorage.ru/", file.FileId);
                    }
                }
            }
            return result;
        }
    }
}
