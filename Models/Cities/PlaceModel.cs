﻿namespace InfoPoster_backend.Models.Cities
{
    public class PlaceRequestModel
    {
        public string Info { get; set; }
        public string PlaceLink { get; set; }
        public string Lang { get; set; }
    }

    public class PlaceModel
    {
        public PlaceModel() { }

        public PlaceModel(PlaceRequestModel request, Guid applicationId)
        {
            ApplicationId = applicationId;
            Info = request.Info;
            Lang = request.Lang;
            PlaceLink = request.PlaceLink;
        }

        public void Update(PlaceRequestModel request, Guid applicationId)
        {
            ApplicationId = applicationId;
            PlaceLink = request.PlaceLink;

            if (string.IsNullOrEmpty(Info) || request.Lang == Lang)
                Info = request.Info;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ApplicationId { get; set; }
        public string Info { get; set; }
        public string PlaceLink { get; set; }
        public string Lang { get; set; }
    }
}
