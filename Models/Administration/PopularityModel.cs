namespace InfoPoster_backend.Models.Administration
{
    public enum POPULARITY_PLACE
    {
        MAIN,
        CATEGORY
    }

    public enum POPULARITY_TYPE
    {
        POSTER,
        ORGANIZATION,
        ARTICLE,
        OFFER,
        BANNER
    }

    public class PopularityModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ApplicationId { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? SubcategoryId { get; set; }
        public POPULARITY_PLACE Place { get; set; }
        public POPULARITY_TYPE Type { get; set; }
        public int Popularity { get; set; }
    }

    public class PopularityResponseModel
    {
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }
        public string Name { get; set; }
        public int Popularity { get; set; }
    }
}
