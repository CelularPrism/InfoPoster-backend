using InfoPoster_backend.Models.Administration;
using InfoPoster_backend.Models.Banner;
using InfoPoster_backend.Models.Contexts;
using Microsoft.EntityFrameworkCore;

namespace InfoPoster_backend.Repos
{
    public class BannerRepository
    {
        private readonly BannerContext _banner;
        public BannerRepository(BannerContext banner)
        {
            _banner = banner;
        }

        public async Task<BannerModel> GetBanner(Guid Id) => 
            await _banner.Banners.FirstOrDefaultAsync(b => b.Id == Id);

        public async Task<List<PopularityModel>> GetPopularity(POPULARITY_PLACE place, int popularity) => 
            await _banner.Popularity.Where(p => p.Place == place && p.Popularity == popularity).ToListAsync();

        public async Task<List<BannerModel>> GetPopularBannerList(POPULARITY_PLACE place, Guid city)
        {
            var popularity = await _banner.Popularity.Where(p => p.Place == place && 
                                                                 p.CityId == city &&
                                                                 p.Type == POPULARITY_TYPE.BANNER).Select(p => p.ApplicationId).ToListAsync();
            var banner = await _banner.Banners.Where(b => popularity.Contains(b.Id)).ToListAsync();

            return banner;
        }

        public async Task Add(BannerModel banner)
        {
            await _banner.Banners.AddAsync(banner);
            await _banner.SaveChangesAsync();
        }

        public async Task RemoveRange(List<BannerModel> banner)
        {
            _banner.Banners.RemoveRange(banner);
            await _banner.SaveChangesAsync();
        }

        public async Task AddPopularity(PopularityModel popularity)
        {
            await _banner.Popularity.AddAsync(popularity);
            await _banner.SaveChangesAsync();
        }

        public async Task RemoveRangePopularity(List<PopularityModel> popularity)
        {
            _banner.Popularity.RemoveRange(popularity);
            await _banner.SaveChangesAsync();
        }
    }
}
