using InfoPoster_backend.Handlers.Administration;
using InfoPoster_backend.Models.Cities;

namespace InfoPoster_backend.Models.Posters
{
    public class PosterFullInfoResponseModel
    {
        public PosterFullInfoResponseModel(PosterModel poster, PosterFullInfoModel fullInfo, PosterMultilangModel multilang)
        {
            Id = fullInfo.PosterId;
            Name = multilang.Name;
            Description = multilang.Description;
            ReleaseDate = poster.ReleaseDate;
            CategoryId = fullInfo.CategoryId;
            Place = multilang.Place;
            City = fullInfo.City;
            TimeStart = fullInfo.TimeStart;
            Price = fullInfo.Price;
            Adress = multilang.Adress;
            PlaceLink = fullInfo.PlaceLink;
            Phone = multilang.Phone;
            SiteLink = multilang.SiteLink;
            AgeRestriction = fullInfo.AgeRestriction;
            Tickets = multilang.Tickets;
            AttachedOrganizationId = fullInfo.OrganizationId;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Place { get; set; }
        public Guid? City { get; set; }
        public string TimeStart { get; set; }
        public double Price { get; set; }
        public string Adress { get; set; }
        public string PlaceLink { get; set; }
        public List<PlaceModel> Parking { get; set; }
        public string Tags { get; set; }
        public string SocialLinks { get; set; }
        public string Phone { get; set; }
        public string SiteLink { get; set; }
        public string AgeRestriction { get; set; }
        public string Tickets { get; set; }
        public List<GetFileResponse> GaleryUrls { get; set; }
        public List<string> VideoUrls { get; set; }
        public Guid? AttachedOrganizationId { get; set; }
        public string AttachedOrganizationName { get; set; }
        public string Contacts { get; set; }
    }
}
