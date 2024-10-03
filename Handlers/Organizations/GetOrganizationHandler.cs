using InfoPoster_backend.Models;
using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Organizations
{
    public class GetOrganizationRequest : IRequest<GetOrganizationResponse>
    {
        public Guid Id { get; set; }
        public string Lang { get; set; }
    }

    public class GetOrganizationResponse
    {
        public Guid OrganizationId { get; set; }
        public string Lang { get; set; }
        public string Name { get; set; }
        public Guid CategoryId { get; set; }
        public Guid SubcategoryId { get; set; }
        public string PriceLevel { get; set; }
        public string Capacity { get; set; }
        public string City { get; set; }
        public string WorkTime { get; set; }
        public string Adress { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string SiteLink { get; set; }
        public string AgeRestriction { get; set; }
        public string SocialLinks { get; set; }
        public string Description { get; set; }
        public string ParkingInfo { get; set; }
        public string ParkingPlace { get; set; }
        public string Phone { get; set; }
        public string ContactName { get; set; }
        public List<string> GaleryUrls { get; set; }
        public List<string> VideoUrls { get; set; }
        public string FirstName { get; set; }
        public string Zalo { get; set; }
        public string Email { get; set; }
        public string ContactDescription { get; set; }
    }

    public class GetOrganizationHandler : IRequestHandler<GetOrganizationRequest, GetOrganizationResponse>
    {
        private readonly OrganizationRepository _repository;
        public GetOrganizationHandler(OrganizationRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetOrganizationResponse> Handle(GetOrganizationRequest request, CancellationToken cancellationToken = default)
        {
            var organization = await _repository.GetOrganization(request.Id);
            if (organization == null)
                return null;

            var result = new GetOrganizationResponse();

            var fullInfo = await _repository.GetOrganizationFullInfo(request.Id);
            var ml = await _repository.GetOrganizationMultilang(request.Id, request.Lang);

            if (fullInfo != null)
            {
                result.PriceLevel = fullInfo.PriceLevel;
                result.Capacity = fullInfo.Capacity;
                result.WorkTime = fullInfo.WorkTime;
                result.latitude = fullInfo.latitude;
                result.longitude = fullInfo.longitude;
                result.AgeRestriction = fullInfo.AgeRestriction;
            }

            if (ml != null)
            {
                result.Name = ml.Name;
                result.Lang = ml.Lang;
                result.Description = ml.Description;
                result.ParkingInfo = ml.ParkingInfo;
                result.ParkingPlace = ml.ParkingPlace;
                result.Phone = ml.Phone;
                result.ContactName = ml.ContactName;
                result.City = ml.City;
                result.Adress = ml.Adress;
                result.SiteLink = ml.SiteLink;
                result.SocialLinks = ml.SocialLinks;
            } else
            {
                result.Lang = request.Lang;
            }

            var files = await _repository.GetFileUrls(request.Id);

            if (files.Count > 0)
            {
                result.GaleryUrls = files.Where(f => f.FileCategory == (int)FILE_CATEGORIES.IMAGE).Select(f => f.URL).ToList();
                result.VideoUrls = files.Where(f => f.FileCategory == (int)FILE_CATEGORIES.VIDEO).Select(f => f.URL).ToList();
            }

            result.OrganizationId = organization.Id;
            result.CategoryId = organization.CategoryId;
            result.SubcategoryId = organization.SubcategoryId;

            return result;
        }
    }
}
