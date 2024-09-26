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
            City = multilang.City;
            TimeStart = fullInfo.TimeStart;
            Price = fullInfo.Price;
            Adress = multilang.Adress;
            latitude = fullInfo.Latitude;
            longitude = fullInfo.Longitude;
            Parking = multilang.Parking;
            ParkingPlace = multilang.ParkingPlace;
            Phone = multilang.Phone;
            SiteLink = multilang.SiteLink;
            AgeRestriction = fullInfo.AgeRestriction;
            GaleryUrls = new List<string>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime ReleaseDate { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Place { get; set; }
        public string City { get; set; }
        public string TimeStart { get; set; }
        public double Price { get; set; }
        public string Adress { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string Parking { get; set; }
        public string ParkingPlace { get; set; }
        public string Tags { get; set; }
        public string SocialLinks { get; set; }
        public string Phone { get; set; }
        public string SiteLink { get; set; }
        public string AgeRestriction { get; set; }
        public List<string> GaleryUrls { get; set; }
        public List<string> VideoUrls { get; set; }
    }
}
