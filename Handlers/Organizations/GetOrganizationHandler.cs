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
        public string SocialLinks { get; set; }
        public string Description { get; set; } = string.Empty;
        public List<PlaceModel> Parking { get; set; } = new List<PlaceModel>();
        public string Phone { get; set; } = string.Empty;
        public string Contacts { get; set; } = string.Empty;
        public string InternalContacts { get; set; } = string.Empty;
        public List<string> VideoUrls { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string ContactDescription { get; set; } = string.Empty;
        public int Status { get; set; }
        public List<Guid> MenuCategories { get; set; }
        public string Comment { get; set; }
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
                result.Adress = !string.IsNullOrEmpty(ml.Adress) ? ml.Adress : string.Empty;
                result.SiteLink = !string.IsNullOrEmpty(ml.SiteLink) ? ml.SiteLink : string.Empty;
                result.WorkTime = !string.IsNullOrEmpty(ml.WorkTime) ? ml.WorkTime : string.Empty;
            } else
            {
                result.Lang = request.Lang;
            }

            if (contact != null)
            {
                result.InternalContacts = !string.IsNullOrEmpty(contact.InternalContacts) ? contact.InternalContacts : string.Empty;
                result.Contacts = !string.IsNullOrEmpty(contact.Contacts) ? contact.Contacts : string.Empty;
            }

            var files = await _repository.GetFileUrls(request.Id);

            if (files.Count > 0)
            {
                result.VideoUrls = files.Where(f => f.FileCategory == (int)FILE_CATEGORIES.VIDEO).Select(f => f.URL).ToList();
                result.SocialLinks = files.Where(f => f.FileCategory == (int)FILE_CATEGORIES.SOCIAL_LINKS).Select(f => f.URL).FirstOrDefault();
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
            var comment = await _repository.GetLastRejectedComment(request.Id);
            if (comment != null)
                result.Comment = comment.Text;

            return result;
        }
    }
}
