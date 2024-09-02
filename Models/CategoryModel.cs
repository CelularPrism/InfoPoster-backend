using System.ComponentModel.DataAnnotations;

namespace InfoPoster_backend.Models
{
    public class CategoryModel
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
