namespace InfoPoster_backend.Models
{
    public enum FILE_CATEGORIES
    {
        IMAGE,
        VIDEO
    }

    public class FileURLModel
    {
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
}
