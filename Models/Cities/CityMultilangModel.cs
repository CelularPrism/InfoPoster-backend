namespace InfoPoster_backend.Models.Cities
{
    public class CityMultilangModel
    {
        public Guid Id { get; set; }
        public Guid CityId { get; set; }
        public string Name { get; set; }
        public string Lang { get; set; }
    }
}
