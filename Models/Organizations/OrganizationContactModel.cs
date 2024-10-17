using InfoPoster_backend.Handlers.Organizations;

namespace InfoPoster_backend.Models.Organizations
{
    public class OrganizationContactModel
    {
        public void Update(SaveOrganizationRequest request) 
        {
            Name = request.FirstName;
            Phone = request.ContactPhone;
            Email = request.Email;
            Zalo = request.Zalo;
            Comment = request.ContactDescription;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrganizationId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Zalo { get; set; }
        public string Comment { get; set; }
    }
}
