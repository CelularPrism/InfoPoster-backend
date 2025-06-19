namespace InfoPoster_backend.Models.Administration
{
    public class ApplicationCategoryModel
    {
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid CategoryId { get; set; }
        public Guid SubcategoryId { get; set; }
    }
}
