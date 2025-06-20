namespace InfoPoster_backend.Models.Administration
{
    public class PopularityModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ApplicationId { get; set; }
        public Guid CategoryId { get; set; }
        public Guid SubcategoryId { get; set; }
        public int Popularity { get; set; }
    }
}
