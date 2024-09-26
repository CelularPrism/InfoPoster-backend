namespace InfoPoster_backend.Models.Posters
{
    public class PosterViewLogModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PosterId { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}
