using InfoPoster_backend.Handlers.Posters;

namespace InfoPoster_backend.Models.Posters
{
    public class PosterMultilangModel
    {
        public PosterMultilangModel() { }

        public void Update(SaveFullInfoPosterRequest fullInfo)
        {
            Lang = fullInfo.Lang;
            Place = fullInfo.Place;
            City = fullInfo.City;
            Name = fullInfo.Name;
            Adress = fullInfo.Adress;
            Description = fullInfo.Description;
            Parking = fullInfo.Parking;
            ParkingPlace = fullInfo.ParkingPlace;
            Phone = fullInfo.Phone;
            SiteLink = fullInfo.SiteLink;
            SocialLinks = fullInfo.SocialLinks;
        }

        public PosterMultilangModel(SaveFullInfoPosterRequest fullInfo) 
        {
            Id = Guid.NewGuid();
            PosterId = fullInfo.PosterId;
            Lang = fullInfo.Lang;
            Place = fullInfo.Place;
            City = fullInfo.City;
            Name = fullInfo.Name;
            Adress = fullInfo.Adress;
            Description = fullInfo.Description;
            Parking = fullInfo.Parking;
            ParkingPlace = fullInfo.ParkingPlace;
            Phone = fullInfo.Phone;
            SiteLink = fullInfo.SiteLink;
            SocialLinks = fullInfo.SocialLinks;
        }

        public Guid Id { get; set; }
        public Guid PosterId { get; set; }
        public string Lang { get; set; }
        public string Place { get; set; }
        public string City { get; set; }
        public string Name { get; set; }
        public string Adress { get; set; }
        public string Description { get; set; }
        public string Parking { get; set; }
        public string ParkingPlace { get; set; }
        public string Phone { get; set; }
        public string SiteLink { get; set; }
        public string SocialLinks { get; set; }
    }
}
