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
        public string Lang { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public Guid SubcategoryId { get; set; }
        public string PriceLevel { get; set; } = string.Empty;
        public string Capacity { get; set; } = string.Empty;
        public Guid? City { get; set; }
        public string WorkTime { get; set; } = string.Empty;
        public string Adress { get; set; } = string.Empty;
        public string PlaceLink { get; set; } = string.Empty;
        public string SiteLink { get; set; } = string.Empty;
        public string AgeRestriction { get; set; } = string.Empty;
        public List<string> SocialLinks { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<PlaceModel> Parking { get; set; } = new List<PlaceModel>();
        public string Phone { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public List<string> VideoUrls { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string Zalo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ContactDescription { get; set; } = string.Empty;
        public int Status { get; set; }
        public List<Guid> MenuCategories { get; set; }
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
            var contact = await _repository.GetContact(request.Id);

            if (fullInfo != null)
            {
                result.PriceLevel = !string.IsNullOrEmpty(fullInfo.PriceLevel)? fullInfo.PriceLevel : string.Empty;
                result.Capacity = !string.IsNullOrEmpty(fullInfo.Capacity) ? fullInfo.Capacity : string.Empty;
                result.WorkTime = !string.IsNullOrEmpty(fullInfo.WorkTime) ? fullInfo.WorkTime : string.Empty;
                result.PlaceLink = !string.IsNullOrEmpty(fullInfo.PlaceLink) ? fullInfo.PlaceLink : string.Empty;
                result.AgeRestriction = !string.IsNullOrEmpty(fullInfo.AgeRestriction) ? fullInfo.AgeRestriction : string.Empty;
                result.City = fullInfo.City;
            }

            if (ml != null)
            {
                result.Name = !string.IsNullOrEmpty(ml.Name) ? ml.Name : string.Empty;
                result.Lang = !string.IsNullOrEmpty(ml.Lang) ? ml.Lang : string.Empty;
                result.Description = !string.IsNullOrEmpty(ml.Description) ? ml.Description : string.Empty;
                result.Phone = !string.IsNullOrEmpty(ml.Phone) ? ml.Phone : string.Empty;
                result.ContactName = !string.IsNullOrEmpty(ml.ContactName) ? ml.ContactName : string.Empty;
                result.Adress = !string.IsNullOrEmpty(ml.Adress) ? ml.Adress : string.Empty;
                result.SiteLink = !string.IsNullOrEmpty(ml.SiteLink) ? ml.SiteLink : string.Empty;
            } else
            {
                result.Lang = request.Lang;
            }

            if (contact != null)
            {
                result.FirstName = !string.IsNullOrEmpty(contact.Name) ? contact.Name : string.Empty;
                result.ContactPhone = !string.IsNullOrEmpty(contact.Phone) ? contact.Phone : string.Empty;
                result.Zalo = !string.IsNullOrEmpty(contact.Zalo) ? contact.Zalo : string.Empty;
                result.Email = !string.IsNullOrEmpty(contact.Email) ? contact.Email : string.Empty;
                result.ContactDescription = !string.IsNullOrEmpty(contact.Comment) ? contact.Comment : string.Empty;
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
