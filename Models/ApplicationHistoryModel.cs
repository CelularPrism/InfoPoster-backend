namespace InfoPoster_backend.Models
{
    public class ApplicationHistoryModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public Guid ApplicationId { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class ApplicationHistoryResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public Guid ApplicationId { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
