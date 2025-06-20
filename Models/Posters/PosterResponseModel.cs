namespace InfoPoster_backend.Models.Posters
{
    public class PosterResponseModel
    {
        public PosterResponseModel(PosterModel poster, PosterMultilangModel multilang)
        {
            Id = poster.Id;
            Name = multilang.Name;
            Description = multilang.Description.Length < 100 ? multilang.Description : multilang.Description.Substring(0, 100);
            CategoryId = poster.CategoryId != null ? (Guid)poster.CategoryId : Guid.Empty;
            SubcategoryId = poster.SubcategoryId != null ? (Guid)poster.SubcategoryId : Guid.Empty;
            Place = multilang.Place;
            ReleaseDate = poster.ReleaseDate;
            ReleaseDateEnd = poster.ReleaseDateEnd;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public DateTime? ReleaseDateEnd { get; set; }
        public Guid CategoryId { get; set; }
        public Guid SubcategoryId { get; set; }
        public string CategoryName { get; set; }
        public string SubcategoryName { get; set; }
        public string Place { get; set; }
        public double Price { get; set; }
        public Guid? FileId { get; set; }
        public string FileURL { get; set; }
    }
}
