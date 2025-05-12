using Microsoft.Extensions.Logging;

namespace InfoPoster_backend.Models.Posters
{
    public class PosterExportFileModel
    {
        public List<PosterArrayModel> Array { get; set; }
    }

    public class PosterArrayModel
    {
        public string Name { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? City { get; set; }
        public DateTime ReleaseDateStart { get; set; }
        public DateTime? ReleaseDateEnd { get; set; }
        public string TimeStart { get; set; }
        public double Price { get; set; }
        public string PlaceLink { get; set; }
        public string AgeRestriction { get; set; }
        public List<PosterLangsModel> Langs { get; set; }
        public string SocialLinks { get; set; }
    }

    public class PosterLangsModel
    {
        public string Name { get; set; }
        public string Place { get; set; }
        public string Adress { get; set; }
        public string Description { get; set; }
        public string Phone { get; set; }
        public string SiteLink { get; set; }
        public string Tickets { get; set; }
        public string Lang { get; set; }
    }
}
