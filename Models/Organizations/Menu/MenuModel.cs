namespace InfoPoster_backend.Models.Organizations.Menu
{
    public class MenuModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
    }

    public class MenuToOrganizationModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrganizationId { get; set; }
        public Guid MenuId { get; set; }
    }
}
