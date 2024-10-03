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
            latitude = model.latitude;
            longitude = model.longitude;
            AgeRestriction = model.AgeRestriction;
        }
        public void Update(SaveOrganizationRequest model)
        {
            PriceLevel = model.PriceLevel;
            Capacity = model.Capacity;
            WorkTime = model.WorkTime;
            latitude = model.latitude;
            longitude = model.longitude;
            AgeRestriction = model.AgeRestriction;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrganizationId { get; set; }
        public string PriceLevel { get; set; }
        public string Capacity { get; set; }
        public string WorkTime { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string AgeRestriction { get; set; }
    }
}
