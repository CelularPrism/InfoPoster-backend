using InfoPoster_backend.Handlers.Organizations;

namespace InfoPoster_backend.Models.Organizations
{
    public class OrganizationMultilangModel
    {
        public OrganizationMultilangModel() { }
        public OrganizationMultilangModel(SaveOrganizationRequest model)
        {
            OrganizationId = model.OrganizationId;
            Lang = model.Lang;
            Name = model.Name;
            Description = model.Description;
            ParkingInfo = model.ParkingInfo;
            ParkingPlace = model.ParkingPlace;
            Phone = model.Phone;
            ContactName = model.ContactName;
        }

        public void Update(SaveOrganizationRequest model)
        {
            Lang = model.Lang;
            Name = model.Name;
            Description = model.Description;
            ParkingInfo = model.ParkingInfo;
            ParkingPlace = model.ParkingPlace;
            Phone = model.Phone;
            ContactName = model.ContactName;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrganizationId { get; set; }
        public string Lang { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ParkingInfo { get; set; }
        public string ParkingPlace { get; set; }
        public string Phone { get; set; }
        public string ContactName { get; set; }
    }
}
