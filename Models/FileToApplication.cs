namespace InfoPoster_backend.Models
{
    public enum FILE_PLACES
    {
        GALLERY,
        ORGANIZATION_MENU
    }

    public class FileToApplication
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid FileId { get; set; }
        public Guid ApplicationId { get; set; }
        public int Place { get; set; } = (int)FILE_PLACES.GALLERY;
        public bool IsPrimary { get; set; }
    }
}
