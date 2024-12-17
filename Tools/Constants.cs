namespace InfoPoster_backend.Tools
{
    public static class Constants
    {
        public const string DefaultLang = "en";
        public static Guid DefaultCity = Guid.Parse("30EE25BA-3101-4E33-BF11-D37566468942");
        public static List<string> SystemLangs = new List<string> { "en", "vn", "ru", "ch", "kr" };
        public const string HTTP_ITEM_ClientLang = "ClientLang";

        public static Guid ROLE_ADMIN = Guid.Parse("C7D65315-0AD4-486F-9BC1-88F86CC1D45B");
        public static Guid ROLE_EDITOR = Guid.Parse("A9251469-BFE8-4073-886C-0570E4732260");
    }
}
