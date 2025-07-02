using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Account;
using InfoPoster_backend.Models.Cities;
using InfoPoster_backend.Models.Contexts;
using InfoPoster_backend.Models.Offers;
using InfoPoster_backend.Models.Organizations;
using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Tools;
using Microsoft.EntityFrameworkCore;

namespace InfoPoster_backend.Repos
{
    public class OfferRepository
    {
        private readonly OfferContext _context;
        private readonly string _lang;

        public OfferRepository(OfferContext context, IHttpContextAccessor accessor)
        {
            _context = context;
            _lang = accessor.HttpContext.Items[Constants.HTTP_ITEM_ClientLang].ToString();
        }

        public async Task<OffersModel> GetOffer(Guid id) => await _context.Offers.FirstOrDefaultAsync(o => o.Id == id);

        public async Task<List<OffersMultilangModel>> GetOfferMultilang(Guid offerId) => await _context.OffersMultilang.Where(o => o.OfferId == offerId).ToListAsync();

        public async Task<OffersMultilangModel> GetOfferMultilang(Guid offerId, string lang) => await _context.OffersMultilang.FirstOrDefaultAsync(o => o.OfferId == offerId && o.Lang == lang);

        public async Task<List<CityModel>> GetCities(string lang) => await _context.CitiesMultilang.Where(c => c.Lang == lang).Select(c => new CityModel()
                                                                                                                                    {
                                                                                                                                        Id = c.CityId,
                                                                                                                                        Name = c.Name
                                                                                                                                    }).ToListAsync();

        public async Task<List<OffersModel>> GetOfferList(List<POSTER_STATUS> statuses, OFFER_TYPES? type, DateTime? startDate, DateTime? endDate, Guid? userId, Guid? cityId)
        {
            var query = _context.Offers.Where(o => statuses.Contains(o.Status));

            query = Filter(query, type, startDate, endDate, userId, cityId);

            var result = await query.ToListAsync();
            return result;
        }

        public async Task<List<OffersMultilangModel>> GetMultilangList(IEnumerable<Guid> offers, string lang) =>
            await _context.OffersMultilang.Where(o => offers.Contains(o.OfferId) && o.Lang == lang).ToListAsync();

        public async Task<List<UserModel>> GetUserList(IEnumerable<Guid> users) =>
            await _context.Users.Where(o => users.Contains(o.Id)).ToListAsync();

        public async Task<RejectedComments> GetLastRejectedComment(Guid offerId) =>
            await _context.RejectedComments.Where(r => r.ApplicationId == offerId).OrderByDescending(r => r.CreatedAt).FirstOrDefaultAsync();

        public async Task AddOffer(OffersModel model)
        {
            await _context.Offers.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateOffer(OffersModel model)
        {
            _context.Offers.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task AddOfferMultilang(OffersMultilangModel model)
        {
            await _context.OffersMultilang.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task AddOfferMultilang(List<OffersMultilangModel> model)
        {
            await _context.OffersMultilang.AddRangeAsync(model);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateOfferMultilang(OffersMultilangModel model)
        {
            _context.OffersMultilang.Update(model);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateOfferMultilang(List<OffersMultilangModel> model)
        {
            _context.OffersMultilang.UpdateRange(model);
            await _context.SaveChangesAsync();
        }

        public async Task AddRejectedComment(RejectedComments model)
        {
            await _context.RejectedComments.AddAsync(model);
            await _context.SaveChangesAsync();
        }

        private IQueryable<OffersModel> Filter(IQueryable<OffersModel> query, OFFER_TYPES? type, DateTime? startDate, DateTime? endDate, Guid? userId, Guid? cityId)
        {
            if (type != null)
            {
                query = query.Where(q => q.Type == type);
            }

            if (startDate != null)
            {
                query = query.Where(q => q.CreatedAt >= startDate);
            }

            if (endDate != null)
            {
                query = query.Where(q => q.CreatedAt <= endDate);
            }

            if (userId != null)
            {
                query = query.Where(q => q.UserId == userId);
            }

            if (cityId != null)
            {
                query = query.Where(q => q.CityId == cityId);
            }
            return query;
        }
    }
}
