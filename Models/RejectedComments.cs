namespace InfoPoster_backend.Models
{
    public class RejectedComments
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ApplicationId { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Text { get; set; }
    }
}
