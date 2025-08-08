using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Administration;
using InfoPoster_backend.Models.Banner;
using InfoPoster_backend.Models.Contexts;
using InfoPoster_backend.Models.Organizations;
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

        public async Task<bool> AnyOrganization(Guid id) =>
            await _banner.Organizations.AnyAsync(o => o.Id == id);

        public async Task<bool> AnyPoster(Guid id) =>
            await _banner.Posters.AnyAsync(o => o.Id == id);

        public async Task<List<BannerModel>> GetPopularBannerList(POPULARITY_PLACE place, Guid city)
        {
            var popularity = await _banner.Popularity.Where(p => p.Place == place && 
                                                                 p.CityId == city &&
                                                                 p.Type == POPULARITY_TYPE.BANNER).Select(p => p.ApplicationId).ToListAsync();
            var banner = await _banner.Banners.Where(b => popularity.Contains(b.Id))
                                              .Select(b => new BannerModel()
                                              {
                                                  ApplicationId = b.ApplicationId,
                                                  Comment = b.Comment,
                                                  ExternalLink = b.Type == CategoryType.PLACE ? string.Concat("application/place/" + b.ApplicationId) : b.Type == CategoryType.EVENT ? string.Concat("application/event/" + b.ApplicationId) : b.ExternalLink,
                                                  Id = b.Id,
                                                  PlaceId = b.PlaceId,
                                                  ReleaseDate = b.ReleaseDate,
                                                  Type = b.Type,
                                                  UserId = b.UserId
                                              }).ToListAsync();

            return banner;
        }

        public async Task<CategoryModel> GetCategoryById(Guid categoryId) =>
            await _banner.Categories.Where(c => c.Id == categoryId).FirstOrDefaultAsync();

        public async Task<SubcategoryModel> GetSubcategoryById(Guid subcategoryId) =>
            await _banner.Subcategories.Where(c => c.Id == subcategoryId).FirstOrDefaultAsync();

        public async Task Add(BannerModel banner)
        {
            await _banner.Banners.AddAsync(banner);
            await _banner.SaveChangesAsync();
        }

        public async Task Update(BannerModel banner)
        {
            _banner.Banners.Update(banner);
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
