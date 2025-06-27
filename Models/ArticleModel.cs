using InfoPoster_backend.Handlers.Administration;
using InfoPoster_backend.Models.Posters;

namespace InfoPoster_backend.Models
{
    public class ArticleModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public POSTER_STATUS Status { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Lang { get; set; }
    }

    public class ArticleResponse
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string Lang { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public int Status { get; set; }
        public List<GetFileResponse> GaleryUrls { get; set; }
    }
}
