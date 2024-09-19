namespace InfoPoster_backend.Models
{
    public class PosterSubcategoryModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PosterId { get; set; }
        public Guid SubcategoryId { get; set; }
    }
}
