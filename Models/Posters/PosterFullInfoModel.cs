namespace InfoPoster_backend.Models.Posters
{
    public class PosterFullInfoModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PosterId { get; set; }
        public Guid CategoryId { get; set; }
        public DateTime Date { get; set; }
        public string TimeStart { get; set; }
        public double Price { get; set; }
        public string Longitude { get; set; }
        public string Latitude { get; set; }
        public string AgeRestriction { get; set; }
    }
}
