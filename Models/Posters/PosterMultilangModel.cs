namespace InfoPoster_backend.Models.Posters
{
    public class PosterMultilangModel
    {
        public Guid Id { get; set; }
        public Guid PosterId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Lang { get; set; }
    }
}
