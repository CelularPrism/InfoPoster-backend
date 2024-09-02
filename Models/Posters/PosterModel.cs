using System.ComponentModel.DataAnnotations;

namespace InfoPoster_backend.Models.Posters
{
    public class PosterModel
    {
        [Key]
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Guid CategoryId { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Place { get; set; }
    }
}
