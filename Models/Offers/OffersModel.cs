using InfoPoster_backend.Models.Posters;

namespace InfoPoster_backend.Models.Offers
{
    public enum OFFER_TYPES
    {
        STOCK,
        SPECIAL_OFFER
    }

    public class OffersModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CityId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime DateStart { get; set; } = DateTime.UtcNow.Date;
        public DateTime? DateEnd { get; set; }
        public Guid UserId { get; set; }
        public OFFER_TYPES Type { get; set; }
        public POSTER_STATUS Status { get; set; }
        public string Name { get; set; }
        //public string PlaceLink { get; set; }
    }
}
