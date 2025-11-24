using InfoPoster_backend.Handlers.Posters;

namespace InfoPoster_backend.Models.Posters
{
    public class PosterMultilangModel
    {
        public PosterMultilangModel() { }

        public List<ApplicationChangeHistory> Update(SaveFullInfoPosterRequest fullInfo, Guid articleId, Guid userId)
        {
            var history = new List<ApplicationChangeHistory>();
            if ((string.IsNullOrEmpty(Place) && fullInfo.Place != "null") || fullInfo.Lang == Lang)
            {
                history.Add(new ApplicationChangeHistory(articleId, fullInfo.PosterId, "Place " + Lang, Place, fullInfo.Place, userId));
                Place = fullInfo.Place;
            }

            if (string.IsNullOrEmpty(Name) || fullInfo.Lang == Lang)
            {
                history.Add(new ApplicationChangeHistory(articleId, fullInfo.PosterId, "Name " + Lang, Name, fullInfo.Name, userId));
                Name = fullInfo.Name;
            }

            if (string.IsNullOrEmpty(Adress) || fullInfo.Lang == Lang)
            {
                history.Add(new ApplicationChangeHistory(articleId, fullInfo.PosterId, "Adress " + Lang, Adress, fullInfo.Adress, userId));
                Adress = fullInfo.Adress;
            }

            if (string.IsNullOrEmpty(Description) || fullInfo.Lang == Lang)
            {
                history.Add(new ApplicationChangeHistory(articleId, fullInfo.PosterId, "Description " + Lang, Description, fullInfo.Description, userId));
                Description = fullInfo.Description;
            }

            if (string.IsNullOrEmpty(Phone) || fullInfo.Lang == Lang)
            {
                history.Add(new ApplicationChangeHistory(articleId, fullInfo.PosterId, "Phone " + Lang, Phone, fullInfo.Phone, userId));
                Phone = fullInfo.Phone;
            }

            if (string.IsNullOrEmpty(Tickets) || fullInfo.Lang == Lang)
            {
                history.Add(new ApplicationChangeHistory(articleId, fullInfo.PosterId, "Tickets " + Lang, Tickets, fullInfo.Tickets, userId));
                Tickets = fullInfo.Tickets;
            }

            history.Add(new ApplicationChangeHistory(articleId, fullInfo.PosterId, "SiteLink " + Lang, SiteLink, fullInfo.SiteLink, userId));
            SiteLink = fullInfo.SiteLink;
            return history;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PosterId { get; set; }
        public string Lang { get; set; }
        public string Place { get; set; }
        public string Name { get; set; }
        public string Adress { get; set; }
        public string Description { get; set; }
        public string Phone { get; set; }
        public string SiteLink { get; set; }
        public string Tickets { get; set; }
    }
}
