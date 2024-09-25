namespace InfoPoster_backend.Models.Posters
{
    public class PosterFullInfoResponseModel
    {
        public PosterFullInfoResponseModel(PosterModel poster, PosterMultilangModel multilang)
        {
            Id = poster.Id;
            Name = !string.IsNullOrEmpty(multilang.Name) ? multilang.Name : poster.Name;
            Description = !string.IsNullOrEmpty(multilang.Description) ? multilang.Description : poster.Description;
            CategoryId = poster.CategoryId;
            Place = poster.Place;
            CategoryName = string.Empty;
            GaleryUrls = new List<string>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime ReleaseDate { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Place { get; set; }
        public List<string> GaleryUrls { get; set; }
    }
}
