using InfoPoster_backend.Handlers.Organizations;
using InfoPoster_backend.Handlers.Posters;

namespace InfoPoster_backend.Models
{
    public class ContactModel
    {
        public void Update(SaveOrganizationRequest request)
        {
            Name = request.FirstName;
            Phone = request.ContactPhone;
            Comment = request.ContactDescription;
        }

        public void Update(SaveFullInfoPosterRequest request)
        {
            Name = request.FirstName;
            Phone = request.ContactPhone;
            Email = request.Email;
            Zalo = request.Zalo;
            Comment = request.ContactDescription;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ApplicationId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Zalo { get; set; }
        public string Comment { get; set; }
    }
}
