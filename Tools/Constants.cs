namespace InfoPoster_backend.Tools
{
    public static class Constants
    {
        public const string DefaultLang = "en";
        public static Guid DefaultCity = Guid.Parse("30EE25BA-3101-4E33-BF11-D37566468942");
        public static List<string> SystemLangs = new List<string> { "en", "vn", "ru", "ch", "kr" };
        public const string HTTP_ITEM_ClientLang = "ClientLang";
    }
}
