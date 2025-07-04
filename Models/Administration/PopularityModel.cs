namespace InfoPoster_backend.Models.Administration
{
    public enum POPULARITY_PLACE
    {
        MAIN,
        CATEGORY
    }

    public class PopularityModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ApplicationId { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? SubcategoryId { get; set; }
        public POPULARITY_PLACE Place { get; set; }
        public int Popularity { get; set; }
    }
}
