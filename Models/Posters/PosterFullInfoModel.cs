using InfoPoster_backend.Handlers.Posters;

namespace InfoPoster_backend.Models.Posters
{
    public class PosterFullInfoModel
    {
        public List<ApplicationChangeHistory> Update(SaveFullInfoPosterRequest request, Guid articleId, Guid userId)
        {
            var history = new List<ApplicationChangeHistory>();
            if (CategoryId != request.CategoryId)
            {
                history.Add(new ApplicationChangeHistory(articleId, request.PosterId, "CategoryId", 
                    CategoryId == Guid.Empty ? null : CategoryId.ToString(), 
                    request.CategoryId == null ? null : request.CategoryId.ToString(), userId));

                CategoryId = request.CategoryId == null ? Guid.Empty : (Guid)request.CategoryId;
            }

            if (City != request.City)
            {
                history.Add(new ApplicationChangeHistory(articleId, request.PosterId, "City",
                    City == null ? null : CategoryId.ToString(),
                    request.City == null ? null : request.City.ToString(), userId));
                City = request.City;
            }

            if (OrganizationId != request.AttachedOrganizationId)
            {
                history.Add(new ApplicationChangeHistory(articleId, request.PosterId, "Organization",
                    OrganizationId == null ? null : OrganizationId.ToString(),
                    request.AttachedOrganizationId == null ? null : request.AttachedOrganizationId.ToString(), userId));
                OrganizationId = request.AttachedOrganizationId;
            }

            if (TimeStart != request.TimeStart)
            {
                history.Add(new ApplicationChangeHistory(articleId, request.PosterId, "TimeStart", TimeStart, request.TimeStart, userId));
                TimeStart = request.TimeStart;
            }

            //if (Price != request.Price)
            //{
            //    history.Add(new ApplicationChangeHistory(articleId, request.PosterId, "Price", Price.ToString(), request.Price.ToString(), userId));
            //    Price = request.Price;
            //}

            if (PlaceLink != request.PlaceLink)
            {
                history.Add(new ApplicationChangeHistory(articleId, request.PosterId, "PlaceLink", PlaceLink, request.PlaceLink, userId));
                PlaceLink = request.PlaceLink;
            }

            if (AgeRestriction != request.AgeRestriction)
            {
                history.Add(new ApplicationChangeHistory(articleId, request.PosterId, "AgeRestriction", AgeRestriction, request.AgeRestriction, userId));
                AgeRestriction = request.AgeRestriction;
            }
            return history;
        }

        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid PosterId { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? City { get; set; }
        public Guid? OrganizationId { get; set; }
        public string TimeStart { get; set; }
        //public double Price { get; set; }
        public string PlaceLink { get; set; }
        public string AgeRestriction { get; set; }
    }
}
