using InfoPoster_backend.Handlers.Posters;

namespace InfoPoster_backend.Models.Posters
{
    public class PosterMultilangModel
    {
        public PosterMultilangModel() { }

        public void Update(SaveFullInfoPosterRequest fullInfo)
        {
            if ((string.IsNullOrEmpty(Place) && fullInfo.Place != "null") || fullInfo.Lang == Lang)
                Place = fullInfo.Place;

            if (string.IsNullOrEmpty(Name) || fullInfo.Lang == Lang)
                Name = fullInfo.Name;

            if (string.IsNullOrEmpty(Adress) || fullInfo.Lang == Lang)
                Adress = fullInfo.Adress;

            if (string.IsNullOrEmpty(Description) || fullInfo.Lang == Lang)
                Description = fullInfo.Description;

            if (string.IsNullOrEmpty(Phone) || fullInfo.Lang == Lang)
                Phone = fullInfo.Phone;

            if (string.IsNullOrEmpty(SiteLink) || fullInfo.Lang == Lang)
                SiteLink = fullInfo.SiteLink;

            if (string.IsNullOrEmpty(Tickets) || fullInfo.Lang == Lang)
                Tickets = fullInfo.Tickets;
        }

        public PosterMultilangModel(SaveFullInfoPosterRequest fullInfo, string lang) 
        {
            Id = Guid.NewGuid();
            PosterId = fullInfo.PosterId;
            Lang = lang;
            Name = fullInfo.Name;
            Adress = fullInfo.Adress;
            Description = fullInfo.Description;
            Phone = fullInfo.Phone;
            SiteLink = fullInfo.SiteLink;
            Tickets = fullInfo.Tickets;

            if ((string.IsNullOrEmpty(Place) && fullInfo.Place != "null") || fullInfo.Lang == Lang)
                Place = fullInfo.Place;
        }

        public Guid Id { get; set; }
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
