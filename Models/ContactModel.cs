using InfoPoster_backend.Handlers.Organizations;
using InfoPoster_backend.Handlers.Posters;

namespace InfoPoster_backend.Models
{
    public class ContactModel
    {
        public List<ApplicationChangeHistory> Update(SaveOrganizationRequest request, Guid articleId, Guid userId)
        {
            var history = new List<ApplicationChangeHistory>();
            if (Contacts != request.Contacts)
            {
                history.Add(new ApplicationChangeHistory(articleId, request.OrganizationId, "Contacts", Contacts, request.Contacts, userId));
                Contacts = request.Contacts;
            }

            if (InternalContacts != request.InternalContacts)
            {
                history.Add(new ApplicationChangeHistory(articleId, request.OrganizationId, "InternalContacts", InternalContacts, request.InternalContacts, userId));
                InternalContacts = request.InternalContacts;
            }

            return history;
        }

        public List<ApplicationChangeHistory> Update(SaveFullInfoPosterRequest request, Guid articleId, Guid userId)
        {
            var history = new List<ApplicationChangeHistory>();
            if (Contacts != request.Contacts)
            {
                history.Add(new ApplicationChangeHistory(articleId, request.PosterId, "Contacts", Contacts, request.Contacts, userId));
                Contacts = request.Contacts;
            }

            if (InternalContacts != request.InternalContacts)
            {
                history.Add(new ApplicationChangeHistory(articleId, request.PosterId, "InternalContacts", InternalContacts, request.InternalContacts, userId));
                InternalContacts = request.InternalContacts == "null" ? null : request.InternalContacts;
            }

            return history;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ApplicationId { get; set; }
        public string Contacts { get; set; } // Contacts (Name)
        public string InternalContacts { get; set; } // InternalContacts (Phone)
        public string Lang { get; set; }
    }
}
