namespace InfoPoster_backend.Models
{
    public class FileToApplication
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid FileId { get; set; }
        public Guid ApplicationId { get; set; }
    }
}
