using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Cities;
using InfoPoster_backend.Models.Organizations.Menu;
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
        public string PlaceLink { get; set; }
        public string SiteLink { get; set; }
        public string AgeRestriction { get; set; }
        public List<string> SocialLinks { get; set; }
        public string Description { get; set; }
        public List<PlaceModel> Parking { get; set; }
        public string Phone { get; set; }
        public string ContactName { get; set; }
        public List<string> VideoUrls { get; set; }
        public string FirstName { get; set; }
        public string Zalo { get; set; }
        public string Email { get; set; }
        public string ContactDescription { get; set; }
        public int Status { get; set; }
        public List<MenuModel> MenuCategories { get; set; }
    }

    public class GetOrganizationHandler : IRequestHandler<GetOrganizationRequest, GetOrganizationResponse>
    {
        private readonly OrganizationRepository _repository;
        private readonly string _lang;

        public GetOrganizationHandler(OrganizationRepository repository, IHttpContextAccessor accessor)
        {
            _repository = repository;
            _lang = accessor.HttpContext.Items["ClientLang"].ToString().ToLower();
        }

        public async Task<GetOrganizationResponse> Handle(GetOrganizationRequest request, CancellationToken cancellationToken = default)
        {
            var organization = await _repository.GetOrganization(request.Id);
            if (organization == null)
                return null;

            var result = new GetOrganizationResponse();
            result.Status = organization.Status;

            var fullInfo = await _repository.GetOrganizationFullInfo(request.Id);
            var ml = await _repository.GetOrganizationMultilang(request.Id, request.Lang);

            if (fullInfo != null)
            {
                result.PriceLevel = fullInfo.PriceLevel;
                result.Capacity = fullInfo.Capacity;
                result.WorkTime = fullInfo.WorkTime;
                result.PlaceLink = fullInfo.PlaceLink;
                result.AgeRestriction = fullInfo.AgeRestriction;
            }

            if (ml != null)
            {
                result.Name = ml.Name;
                result.Lang = ml.Lang;
                result.Description = ml.Description;
                result.Phone = ml.Phone;
                result.ContactName = ml.ContactName;
                result.City = ml.City;
                result.Adress = ml.Adress;
                result.SiteLink = ml.SiteLink;
            } else
            {
                result.Lang = request.Lang;
            }

            var files = await _repository.GetFileUrls(request.Id);

            if (files.Count > 0)
            {
                result.VideoUrls = files.Where(f => f.FileCategory == (int)FILE_CATEGORIES.VIDEO).Select(f => f.URL).ToList();
                result.SocialLinks = files.Where(f => f.FileCategory == (int)FILE_CATEGORIES.SOCIAL_LINKS).Select(f => f.URL).ToList();
            }

            var places = await _repository.GetPlaces(request.Id);
            if (places.Count > 0)
            {
                result.Parking = places;
            }

            result.MenuCategories = await _repository.GetMenuList(request.Id, _lang);

            result.OrganizationId = organization.Id;
            result.CategoryId = organization.CategoryId;
            result.SubcategoryId = organization.SubcategoryId;

            return result;
        }
    }
}
