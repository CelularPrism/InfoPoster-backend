namespace InfoPoster_backend.Models.Organizations.Menu
{
    public class MenuMultilangModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid MenuId { get; set; }
        public string Lang { get; set; }
        public string Name { get; set; }
    }
}
