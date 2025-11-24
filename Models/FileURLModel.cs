namespace InfoPoster_backend.Models
{
    public enum FILE_CATEGORIES
    {
        IMAGE,
        VIDEO,
        SOCIAL_LINKS,
        YOUTUBE,
        FACEBOOK,
        INSTAGRAM,
        TIKTOK
    }

    public class FileURLModel
    {
        public FileURLModel() { }
        public FileURLModel(Guid posterId, string url, int category)
        {
            PosterId = posterId;
            URL = url;
            FileCategory = category;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PosterId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string URL { get; set; }
        public int FileCategory { get; set; }
    }

    public class OrganizationFileURLModel
    {
        public OrganizationFileURLModel() { }
        public OrganizationFileURLModel(Guid organizationId, string url, int category)
        {
            OrganizationId = organizationId;
            URL = url;
            FileCategory = category;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrganizationId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string URL { get; set; }
        public int FileCategory { get; set; }
    }
}
