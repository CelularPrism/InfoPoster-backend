using System.ComponentModel.DataAnnotations;

namespace InfoPoster_backend.Models.Posters
{
    public enum POSTER_STATUS
    {
        PENDING,
        DRAFT,
        DELETED,
        PUBLISHED,
        REJECTED,
        REVIEWING
    }

    public class PosterModel
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string Name { get; set; }
        public Guid CategoryId { get; set; }
        public int Status { get; set; }
        public Guid UserId { get; set; }
    }
}
