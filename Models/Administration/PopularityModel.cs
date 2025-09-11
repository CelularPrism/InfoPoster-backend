namespace InfoPoster_backend.Models.Administration
{
    public enum POPULARITY_PLACE
    {
        MAIN,
        CATEGORY_PLACE,
        SUBCATEGORY_PLACE,
        CATEGORY_EVENT,
        SUBCATEGORY_EVENT,
        LIST_APPLICATION_EVENT,
        LIST_APPLICATION_PLACE,
        LIST_APPLICATION_ARTICLE
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
        public Guid CityId { get; set; }
        public POPULARITY_PLACE Place { get; set; }
        public POPULARITY_TYPE Type { get; set; }
        public int Popularity { get; set; }
        public Guid? PlaceId { get; set; }
    }

    public class PopularityResponseModel
    {
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }
        public string Name { get; set; }
        public int Popularity { get; set; }
    }
}
