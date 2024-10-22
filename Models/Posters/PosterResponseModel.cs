namespace InfoPoster_backend.Models.Posters
{
    public class PosterResponseModel
    {
        public PosterResponseModel(PosterModel poster, PosterMultilangModel multilang)
        {
            Id = poster.Id;
            Name = multilang.Name;
            CategoryId = poster.CategoryId;
            Place = multilang.Place;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime ReleaseDate { get; set; }
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Place { get; set; }
    }
}
