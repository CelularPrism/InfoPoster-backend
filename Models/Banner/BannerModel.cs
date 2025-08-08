namespace InfoPoster_backend.Models.Banner
{
    public class BannerModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string ExternalLink { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Comment { get; set; }
        public Guid? ApplicationId { get; set; }
        public Guid? PlaceId { get; set; }
        public CategoryType? Type { get; set; }
    }

    public class BannerResponseModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string ExternalLink { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Comment { get; set; }
        public string FileURL { get; set; }
        public Guid? ApplicationId { get; set; }
        public CategoryType? Type { get; set; }
    }
}
