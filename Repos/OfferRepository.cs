using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Account;
using InfoPoster_backend.Models.Administration;
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
        private readonly Guid _city;

        public OfferRepository(OfferContext context, IHttpContextAccessor accessor)
        {
            _context = context;
            _lang = accessor.HttpContext.Items[Constants.HTTP_ITEM_ClientLang].ToString();
            _city = Guid.TryParse(accessor.HttpContext.Request.Headers["X-Testing"].ToString(), out _city) ? Guid.Parse(accessor.HttpContext.Request.Headers["X-Testing"].ToString()) : Constants.DefaultCity;
        }

        public async Task<OffersModel> GetOffer(Guid id) => await _context.Offers.FirstOrDefaultAsync(o => o.Id == id);

        public async Task<List<OffersMultilangModel>> GetOfferMultilang(Guid offerId) => await _context.OffersMultilang.Where(o => o.OfferId == offerId).ToListAsync();

        public async Task<OffersMultilangModel> GetOfferMultilang(Guid offerId, string lang) => await _context.OffersMultilang.FirstOrDefaultAsync(o => o.OfferId == offerId && o.Lang == lang);

        public async Task<List<OffersModel>> GetOfferList() => await _context.Offers.Where(o => o.Status == POSTER_STATUS.PUBLISHED)
                                                                                    .Join(_context.OffersMultilang,
                                                                                          offer => offer.Id,
                                                                                          ml => ml.OfferId,
                                                                                          (offer, ml) => new { Offer = offer, Multilang = ml })
                                                                                    .Where(o => o.Multilang.Lang == _lang)
                                                                                    .Select(o => new OffersModel()
                                                                                    {
                                                                                        CityId = o.Offer.CityId,
                                                                                        CreatedAt = o.Offer.CreatedAt,
                                                                                        DateEnd = o.Offer.DateEnd,
                                                                                        DateStart = o.Offer.DateStart,
                                                                                        Id = o.Offer.Id,
                                                                                        Name = o.Multilang.Name,
                                                                                        Status = o.Offer.Status,
                                                                                        Type = o.Offer.Type,
                                                                                        UserId = o.Offer.UserId
                                                                                    }).ToListAsync();                                         

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

        public async Task<List<OffersResponseModel>> GetOffersByCity() =>
            await _context.Offers.Where(o => o.Status == POSTER_STATUS.PUBLISHED && 
                                             (o.DateEnd > DateTime.UtcNow.Date || o.DateEnd == null) && 
                                             o.CityId == _city)
                                 .Join(_context.OffersMultilang,
                                       o => o.Id,
                                       ml => ml.OfferId,
                                       (o, ml) => new { Offer = o, Multilang = ml })
                                 .Where(o => o.Multilang.Lang == _lang)
                                 .Select(o => new OffersResponseModel()
                                 {
                                    DateEnd = o.Offer.DateEnd,
                                    DateStart = o.Offer.DateStart,
                                    Description = o.Multilang.Description,
                                    Id = o.Offer.Id,
                                    Name = o.Multilang.Name,
                                    Type = o.Offer.Type
                                 })
                                 .OrderBy(o => o.DateStart)
                                 .ToListAsync();

        public async Task<OffersResponseModel> GetOfferResponse(Guid id) =>
            await _context.Offers.Where(o => o.Id == id)
                                 .Join(_context.OffersMultilang,
                                       o => o.Id,
                                       ml => ml.OfferId,
                                       (o, ml) => new { Offer = o, Multilang = ml })
                                 .Where(o => o.Multilang.Lang == _lang)
                                 .Select(o => new OffersResponseModel()
                                 {
                                     DateEnd = o.Offer.DateEnd,
                                     DateStart = o.Offer.DateStart,
                                     Description = o.Multilang.Description,
                                     Id = o.Offer.Id,
                                     Name = o.Multilang.Name,
                                     Type = o.Offer.Type
                                 })
                                 .OrderBy(o => o.DateStart)
                                 .FirstOrDefaultAsync();

        public async Task<List<OffersMultilangModel>> GetMultilangList(IEnumerable<Guid> offers, string lang) =>
            await _context.OffersMultilang.Where(o => offers.Contains(o.OfferId) && o.Lang == lang).ToListAsync();

        public async Task<List<UserModel>> GetUserList(IEnumerable<Guid> users) =>
            await _context.Users.Where(o => users.Contains(o.Id)).ToListAsync();

        public async Task<RejectedComments> GetLastRejectedComment(Guid offerId) =>
            await _context.RejectedComments.Where(r => r.ApplicationId == offerId).OrderByDescending(r => r.CreatedAt).FirstOrDefaultAsync();

        public async Task<List<PopularityModel>> GetPopularityList(POPULARITY_PLACE place)
        {
            var publishedOrgs = await _context.Offers.Where(o => o.Status == POSTER_STATUS.PUBLISHED).Select(o => o.Id).ToListAsync();
            var result = await _context.Popularity.Where(p => publishedOrgs.Contains(p.ApplicationId) && p.Place == place).ToListAsync();
            return result;
        }

        public async Task<List<OffersModel>> GetPopularPosterList(POPULARITY_PLACE place)
        {
            var popularityOffers = await _context.Popularity.Where(p => p.Place == place).OrderBy(p => p.Popularity).Select(p => p.ApplicationId).ToListAsync();

            var offerList = await _context.OffersMultilang.Where(ml => ml.Lang == _lang && popularityOffers.Contains(ml.OfferId))
                                                                   .Join(_context.Offers,
                                                                   ml => ml.OfferId,
                                                                   p => p.Id,
                                                                   (ml, p) => new OffersModel()
                                                                   {
                                                                       Id = ml.OfferId,
                                                                       Name = ml.Name,
                                                                       CityId = p.CityId,
                                                                       CreatedAt = p.CreatedAt,
                                                                       DateEnd = p.DateEnd,
                                                                       DateStart = p.DateStart,
                                                                       Status = p.Status,
                                                                       Type = p.Type,
                                                                       UserId = p.UserId
                                                                   }).ToListAsync();
            var result = new List<OffersModel>();
            foreach (var item in popularityOffers)
            {
                var offer = offerList.Where(o => o.Id == item).FirstOrDefault();
                if (offer != null)
                    result.Add(offer);
            }

            return result;
        }

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

        public async Task AddPopularity(PopularityModel popularity)
        {
            await _context.Popularity.AddAsync(popularity);
            await _context.SaveChangesAsync();
        }

        public async Task AddPopularity(List<PopularityModel> popularity)
        {
            await _context.Popularity.AddRangeAsync(popularity);
            await _context.SaveChangesAsync();
        }

        public async Task RemovePopularity(PopularityModel popularity)
        {
            _context.Popularity.Remove(popularity);
            await _context.SaveChangesAsync();
        }

        public async Task RemovePopularity(List<PopularityModel> popularity)
        {
            _context.Popularity.RemoveRange(popularity);
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
