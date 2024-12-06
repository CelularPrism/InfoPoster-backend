using InfoPoster_backend.Handlers.Organizations;

namespace InfoPoster_backend.Models.Organizations
{
    public class OrganizationMultilangModel
    {
        public OrganizationMultilangModel() { }

        public List<ApplicationChangeHistory> Update(SaveOrganizationRequest model, Guid articleId, Guid userId)
        {
            var history = new List<ApplicationChangeHistory>();
            if (string.IsNullOrEmpty(Name) || model.Lang == Lang)
            {
                history.Add(new ApplicationChangeHistory(articleId, model.OrganizationId, "Name " + Lang, Name, model.Name, userId));
                Name = model.Name;
            }

            if (string.IsNullOrEmpty(Description) || model.Lang == Lang)
            {
                history.Add(new ApplicationChangeHistory(articleId, model.OrganizationId, "Description " + Lang, Description, model.Description, userId));
                Description = model.Description;
            }

            if (string.IsNullOrEmpty(Phone) || model.Lang == Lang)
            {
                history.Add(new ApplicationChangeHistory(articleId, model.OrganizationId, "Phone " + Lang, Phone, model.InternalContacts, userId));
                Phone = model.InternalContacts;
            }

            if (string.IsNullOrEmpty(ContactName) || model.Lang == Lang)
            {
                history.Add(new ApplicationChangeHistory(articleId, model.OrganizationId, "ContactName " + Lang, ContactName, model.Contacts, userId));
                ContactName = model.Contacts;
            }

            if (string.IsNullOrEmpty(SiteLink) || model.Lang == Lang)
            {
                history.Add(new ApplicationChangeHistory(articleId, model.OrganizationId, "SiteLink " + Lang, SiteLink, model.SiteLink, userId));
                SiteLink = model.SiteLink;
            }

            if (string.IsNullOrEmpty(Adress) || model.Lang == Lang)
            {
                history.Add(new ApplicationChangeHistory(articleId, model.OrganizationId, "Adress " + Lang, Adress, model.Adress, userId));
                Adress = model.Adress;
            }

            if (string.IsNullOrEmpty(WorkTime) || model.Lang == Lang)
            {
                history.Add(new ApplicationChangeHistory(articleId, model.OrganizationId, "WorkTime " + Lang, WorkTime, model.WorkTime, userId));
                WorkTime = model.WorkTime;
            }

            return history;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrganizationId { get; set; }
        public string Lang { get; set; }
        public string Name { get; set; }
        public string Adress { get; set; }
        public string SiteLink { get; set; }
        public string Description { get; set; }
        public string Phone { get; set; }
        public string ContactName { get; set; }
        public string WorkTime { get; set; }
    }
}
