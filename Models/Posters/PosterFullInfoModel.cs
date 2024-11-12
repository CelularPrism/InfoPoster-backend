namespace InfoPoster_backend.Models.Posters
{
    public class PosterFullInfoModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PosterId { get; set; }
        public Guid CategoryId { get; set; }
        public Guid? City { get; set; }
        public Guid? OrganizationId { get; set; }
        public string TimeStart { get; set; }
        public double Price { get; set; }
        public string PlaceLink { get; set; }
        public string AgeRestriction { get; set; }
    }
}
