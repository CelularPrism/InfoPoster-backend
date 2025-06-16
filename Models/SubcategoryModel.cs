using System.ComponentModel.DataAnnotations;

namespace InfoPoster_backend.Models
{
    public class SubcategoryModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CategoryId { get; set; }
        public string Name { get; set; }
        public string ImageSrc { get; set; }
    }

    public class SubcategoryMultilangModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SubcategoryId { get; set; }
        public string Name { get; set; }
        public string lang { get; set; }
    }

    public class SubcategoryResponseModel
    {
        public string CategoryName { get; set; }
        public List<SubcategoryPopularModel> Subcategories { get; set; }
    }

    public class SubcategoryPopularModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CategoryId { get; set; }
        public string Name { get; set; }
        public string ImageSrc { get; set; }
        public int CountApplications { get; set; }
    }
}
