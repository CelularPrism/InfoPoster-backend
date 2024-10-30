﻿using InfoPoster_backend.Handlers.Organizations;

namespace InfoPoster_backend.Models.Organizations
{
    public class OrganizationMultilangModel
    {
        public OrganizationMultilangModel() { }
        public OrganizationMultilangModel(SaveOrganizationRequest model, string lang)
        {
            OrganizationId = model.OrganizationId;
            Lang = lang;
            Name = model.Name;
            Description = model.Description;
            Phone = model.Phone;
            ContactName = model.ContactName;
            SiteLink = model.SiteLink;
            Adress = model.Adress;
        }

        public void Update(SaveOrganizationRequest model)
        {
            if (string.IsNullOrEmpty(model.Name) || model.Lang == Lang)
                Name = model.Name;

            if (string.IsNullOrEmpty(model.Description) || model.Lang == Lang)
                Description = model.Description;

            if (string.IsNullOrEmpty(model.Phone) || model.Lang == Lang)
                Phone = model.Phone;

            if (string.IsNullOrEmpty(model.ContactName) || model.Lang == Lang)
                ContactName = model.ContactName;

            if (string.IsNullOrEmpty(model.SiteLink) || model.Lang == Lang)
                SiteLink = model.SiteLink;

            if (string.IsNullOrEmpty(model.Adress) || model.Lang == Lang)
                Adress = model.Adress;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrganizationId { get; set; }
        public string Lang { get; set; }
        public string Name { get; set; }
        public string Adress { get; set; }
        public string SiteLink { get; set; }
        public string SocialLinks { get; set; }
        public string Description { get; set; }
        public string ParkingInfo { get; set; }
        public string ParkingPlace { get; set; }
        public string Phone { get; set; }
        public string ContactName { get; set; }
    }
}
