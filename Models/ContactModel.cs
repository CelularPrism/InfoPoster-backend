using InfoPoster_backend.Handlers.Organizations;
using InfoPoster_backend.Handlers.Posters;

namespace InfoPoster_backend.Models
{
    public class ContactModel
    {
        public void Update(SaveOrganizationRequest request)
        {
            Name = request.Contacts;
            Phone = request.InternalContacts;
        }

        public void Update(SaveFullInfoPosterRequest request)
        {
            Name = request.Contacts;
            Phone = request.InternalContacts;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ApplicationId { get; set; }
        public string Name { get; set; } // Contacts
        public string Phone { get; set; } // InternalContacts
        public string Email { get; set; }
        public string Zalo { get; set; }
        public string Comment { get; set; }
    }
}
