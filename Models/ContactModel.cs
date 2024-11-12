using InfoPoster_backend.Handlers.Organizations;
using InfoPoster_backend.Handlers.Posters;

namespace InfoPoster_backend.Models
{
    public class ContactModel
    {
        public void Update(SaveOrganizationRequest request)
        {
            Contacts = request.Contacts;
            InternalContacts = request.InternalContacts;
        }

        public void Update(SaveFullInfoPosterRequest request)
        {
            Contacts = request.Contacts;
            InternalContacts = request.InternalContacts == "null" ? null : request.InternalContacts;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ApplicationId { get; set; }
        public string Contacts { get; set; } // Contacts (Name)
        public string InternalContacts { get; set; } // InternalContacts (Phone)
        public string Lang { get; set; }
    }
}
