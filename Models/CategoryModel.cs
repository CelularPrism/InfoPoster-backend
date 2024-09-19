using System.ComponentModel.DataAnnotations;

namespace InfoPoster_backend.Models
{
    public enum CategoryType
    {
        PLACE,
        EVENT
    }

    public class CategoryModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public int Type { get; set; }
        public string ImageSrc { get; set; }
    }

    public class CategoryMultilangModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CategoryId { get; set; }
        public string Name { get; set; }
        public string lang { get; set; }
    }

    public class CategoryResponseModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public string ImageSrc { get; set; }
    }
}
