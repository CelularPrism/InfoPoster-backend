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

        public async Task<List<PopularityModel>> GetPopularity(POPULARITY_PLACE place) =>
            await _banner.Popularity.Where(p => p.Place == place).ToListAsync();

        public async Task<bool> AnyOrganization(Guid id) =>
            await _banner.Organizations.AnyAsync(o => o.Id == id);

        public async Task<bool> AnyPoster(Guid id) =>
            await _banner.Posters.AnyAsync(o => o.Id == id);

        public async Task<List<BannerModel>> GetPopularBannerList(POPULARITY_PLACE place, Guid city, Guid? placeId)
        {
            var popularity = _banner.Popularity.Where(p => p.Place == place && 
                                                           p.CityId == city &&
                                                           p.Type == POPULARITY_TYPE.BANNER).OrderBy(p => p.Popularity).Select(p => new { p.ApplicationId, p.Popularity }).AsEnumerable();

            var banner = new List<BannerModel>();
            if (popularity.Count() > 0)
            {
                banner = await _banner.Banners.Where(b => popularity.Select(p => p.ApplicationId).Contains(b.Id) && b.PlaceId == placeId)
                                                  .Select(b => new BannerModel()
                                                  {
                                                      Comment = b.Comment,
                                                      ExternalLink = b.ExternalLink,
                                                      Id = b.Id,
                                                      PlaceId = b.PlaceId,
                                                      ReleaseDate = b.ReleaseDate,
                                                      UserId = b.UserId,
                                                      Popularity = popularity.Where(p => p.ApplicationId == b.Id).Select(p => p.Popularity).FirstOrDefault()
                                                  }).OrderBy(p => p.Popularity).ToListAsync();
            }

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
