using InfoPoster_backend.Models.Administration;
using InfoPoster_backend.Models.Banner;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Login;
using MediatR;

namespace InfoPoster_backend.Handlers.Administration.Banner
{
    public class AddPopularityBannerRequest : IRequest<AddPopularityBannerResponse>
    {
        public string ExternalLink { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Comment { get; set; }
        public int Popularity { get; set; }
        public Guid CityId { get; set; }
    }

    public class AddPopularityBannerResponse 
    {
        public Guid Id { get; set; }
    }

    public class AddPopularityBannerHandler : IRequestHandler<AddPopularityBannerRequest, AddPopularityBannerResponse>
    {
        private readonly BannerRepository _repository;
        private readonly Guid _user;
        public AddPopularityBannerHandler(BannerRepository repository, LoginService loginService)
        {
            _repository = repository;
            _user = loginService.GetUserId();
        }

        public async Task<AddPopularityBannerResponse> Handle(AddPopularityBannerRequest request, CancellationToken cancellationToken = default)
        {
            var banner = new BannerModel()
            {
                ExternalLink = request.ExternalLink,
                ReleaseDate = request.ReleaseDate,
                Comment = request.Comment,
                Id = Guid.NewGuid(),
                UserId = _user
            };

            var oldPopularity = await _repository.GetPopularity(POPULARITY_PLACE.MAIN, request.Popularity);

            var popularity = new PopularityModel()
            {
                Id = Guid.NewGuid(),
                ApplicationId = banner.Id,
                Place = POPULARITY_PLACE.MAIN,
                Popularity = request.Popularity,
                Type = POPULARITY_TYPE.BANNER,
                CityId = request.CityId
            };

            await _repository.Add(banner);
            await _repository.AddPopularity(popularity);
            await _repository.RemoveRangePopularity(oldPopularity);

            return new AddPopularityBannerResponse() { Id = banner.Id };
        }
    }
}
