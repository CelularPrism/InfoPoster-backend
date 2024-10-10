namespace InfoPoster_backend.Models.Selectel
{
    public class SelectelFileURLModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Type { get; set; }
    }
}
