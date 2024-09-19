namespace InfoPoster_backend.Models.Posters
{
    public class PosterResponseModel
    {
        public PosterResponseModel(PosterModel poster, PosterMultilangModel multilang)
        {
            Id = poster.Id;
            Name = !string.IsNullOrEmpty(multilang.Name) ? multilang.Name : poster.Name;
            ReleaseDate = poster.ReleaseDate;
            CategoryId = poster.CategoryId;
            Place = poster.Place;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime ReleaseDate { get; set; }
        public Guid CategoryId { get; set; }
        public string Place { get; set; }
    }
}
