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
            WorkTime = model.WorkTime;
            PlaceLink = model.PlaceLink;
            AgeRestriction = model.AgeRestriction;
            City = model.City;
        }
        public void Update(SaveOrganizationRequest model)
        {
            PriceLevel = model.PriceLevel;
            Capacity = model.Capacity;
            WorkTime = model.WorkTime;
            PlaceLink = model.PlaceLink;
            AgeRestriction = model.AgeRestriction;
            City = model.City;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrganizationId { get; set; }
        public Guid? City { get; set; }
        public string PriceLevel { get; set; }
        public string Capacity { get; set; }
        public string WorkTime { get; set; }
        public string PlaceLink { get; set; }
        public string AgeRestriction { get; set; }
    }
}
