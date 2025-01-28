namespace InfoPoster_backend.Handlers.Account
{
    public class GetRecentlyAddedApplicationsResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public string FileURL { get; set; }
    }
}
