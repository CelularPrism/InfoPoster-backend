namespace InfoPoster_backend.Models
{
    public class RoleModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class UserToRolesModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }
}
