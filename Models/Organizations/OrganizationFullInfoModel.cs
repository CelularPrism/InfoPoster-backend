using InfoPoster_backend.Handlers.Organizations;

namespace InfoPoster_backend.Models.Organizations
{
    public class OrganizationFullInfoModel
    {
        public OrganizationFullInfoModel() { }

        public OrganizationFullInfoModel(SaveOrganizationRequest model)
        {
            OrganizationId = model.OrganizationId;
            PriceLevel = model.PriceLevel;
            Capacity = model.Capacity;
            PlaceLink = model.PlaceLink;
            AgeRestriction = model.AgeRestriction;
            City = model.City;
        }

        public List<ApplicationChangeHistory> Update(SaveOrganizationRequest model, Guid articleId, Guid userId)
        {
            var history = new List<ApplicationChangeHistory>();
            if (PriceLevel != model.PriceLevel)
            {
                history.Add(new ApplicationChangeHistory(articleId, model.OrganizationId, "PriceLevel", PriceLevel, model.PriceLevel, userId));
                PriceLevel = model.PriceLevel;
            }

            if (Capacity != model.Capacity)
            {
                history.Add(new ApplicationChangeHistory(articleId, model.OrganizationId, "Capacity", Capacity, model.Capacity, userId));
                Capacity = model.Capacity;
            }

            if (PlaceLink != model.PlaceLink)
            {
                history.Add(new ApplicationChangeHistory(articleId, model.OrganizationId, "PlaceLink", PlaceLink, model.PlaceLink, userId));
                PlaceLink = model.PlaceLink;
            }

            if (AgeRestriction != model.AgeRestriction)
            {
                history.Add(new ApplicationChangeHistory(articleId, model.OrganizationId, "AgeRestriction", AgeRestriction, model.AgeRestriction, userId));
                AgeRestriction = model.AgeRestriction;
            }

            if (City != model.City)
            {
                history.Add(new ApplicationChangeHistory(articleId, model.OrganizationId, "City", City == null ? null : City.ToString(), model.City == null ? null : model.City.ToString(), userId));
                City = model.City;
            }

            return history;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrganizationId { get; set; }
        public Guid? City { get; set; }
        public string PriceLevel { get; set; }
        public string Capacity { get; set; }
        public string PlaceLink { get; set; }
        public string AgeRestriction { get; set; }
    }
}
