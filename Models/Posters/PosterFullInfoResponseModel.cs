namespace InfoPoster_backend.Models.Posters
{
    public class PosterFullInfoResponseModel
    {
        public PosterFullInfoResponseModel(PosterFullInfoModel poster, PosterMultilangModel multilang)
        {
            Id = poster.PosterId;
            Name = multilang.Name;
            Description = multilang.Description;
            ReleaseDate = poster.Date;
            CategoryId = poster.CategoryId;
            Place = multilang.Place;
            City = multilang.City;
            TimeStart = poster.TimeStart;
            Price = poster.Price;
            Adress = multilang.Adress;
            latitude = poster.Latitude;
            longitude = poster.Longitude;
            Parking = multilang.Parking;
            ParkingPlace = multilang.ParkingPlace;
            Phone = multilang.Phone;
            SiteLink = multilang.SiteLink;
            AgeRestriction = poster.AgeRestriction;
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
