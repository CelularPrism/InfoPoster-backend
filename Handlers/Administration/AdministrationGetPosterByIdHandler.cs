using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Cities;
using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Selectel_API;
using InfoPoster_backend.Tools;
using MediatR;
using System.Text.Json.Serialization;

namespace InfoPoster_backend.Handlers.Administration
{
    public class AdministrationGetPosterByIdRequest : IRequest<AdministrationGetPosterByIdResponse>
    {
        public Guid Id { get; set; }
        public string Lang { get; set; }
    }

    public class AdministrationGetPosterByIdResponse
    {
        public Guid PosterId { get; set; }
        public string Lang { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [JsonConverter(typeof(OnlyDateConverter))]
        public DateTime? ReleaseDate { get; set; }
        public Guid CategoryId { get; set; }
        public string Place { get; set; }
        public Guid? City { get; set; }
        public string TimeStart { get; set; }
        public double Price { get; set; }
        public string Adress { get; set; }
        public string PlaceLink { get; set; }
        public List<PlaceModel> Parking { get; set; }
        public string Tags { get; set; }
        public string SocialLinks { get; set; }
        public string SiteLink { get; set; }
        public string AgeRestriction { get; set; }
        public List<string> VideoUrls { get; set; }
        public Guid? AttachedOrganizationId { get; set; }
        public string AttachedOrganizationName { get; set; }
        public string Tickets { get; set; }
        public string Contacts { get; set; }
        public string InternalContacts { get; set; }
        public int Status { get; set; }
        public string Comment { get; set; }
    }

    public class AdministrationGetPosterByIdHandler : IRequestHandler<AdministrationGetPosterByIdRequest, AdministrationGetPosterByIdResponse>
    {
        private readonly PosterRepository _repository;
        private readonly OrganizationRepository _organization;

        public AdministrationGetPosterByIdHandler(PosterRepository repository, OrganizationRepository organization)
        {
            _repository = repository;
            _organization = organization;
        }

        public async Task<AdministrationGetPosterByIdResponse> Handle(AdministrationGetPosterByIdRequest request, CancellationToken cancellationToken = default)
        {
            var poster = await _repository.GetPoster(request.Id);
            if (poster == null)
                return null;

            var result = new AdministrationGetPosterByIdResponse();
            result.Status = poster.Status;

            var fullInfo = await _repository.GetFullInfoPoster(request.Id);
            if (fullInfo != null)
            {
                if (fullInfo.OrganizationId != null)
                {
                    var organization = await _organization.GetOrganization((Guid)fullInfo.OrganizationId);
                    result.AttachedOrganizationName = organization != null ? organization.Name : string.Empty;
                    result.AttachedOrganizationId = organization.Id;
                }

                result.AgeRestriction = !string.IsNullOrEmpty(fullInfo.AgeRestriction) ? fullInfo.AgeRestriction : string.Empty;
                result.CategoryId = fullInfo.CategoryId;
                result.PlaceLink = !string.IsNullOrEmpty(fullInfo.PlaceLink) ? fullInfo.PlaceLink : string.Empty;
                result.TimeStart = !string.IsNullOrEmpty(fullInfo.TimeStart) ? fullInfo.TimeStart : string.Empty;
                result.PosterId = fullInfo.PosterId;
                result.Price = fullInfo.Price;
                result.City = fullInfo.City;
            } else
            {
                result.PosterId = poster.Id;
            }

            var ml = await _repository.GetMultilangPoster(request.Id, request.Lang);
            if (ml != null)
            {
                result.Adress = !string.IsNullOrEmpty(ml.Adress) ? ml.Adress : string.Empty;
                result.Lang = !string.IsNullOrEmpty(ml.Lang) ? ml.Lang : string.Empty;
                result.Description = !string.IsNullOrEmpty(ml.Description) ? ml.Description : string.Empty;
                result.Name = !string.IsNullOrEmpty(ml.Name) ? ml.Name : string.Empty;
                result.Place = !string.IsNullOrEmpty(ml.Place) ? ml.Place : string.Empty;
                result.SiteLink = !string.IsNullOrEmpty(ml.SiteLink) ? ml.SiteLink : string.Empty;
                result.Tickets = !string.IsNullOrEmpty(ml.Tickets) ? ml.Tickets : string.Empty;
            } else
            {
                result.Lang = request.Lang;
            }

            var contact = await _repository.GetContact(request.Id);
            if (contact != null)
            {
                result.Contacts = !string.IsNullOrEmpty(contact.Contacts) ? contact.Contacts : string.Empty;
                result.InternalContacts = !string.IsNullOrEmpty(contact.InternalContacts) ? contact.InternalContacts : string.Empty;
            }
            
            var files = await _repository.GetFileUrls(request.Id);

            result.ReleaseDate = poster.ReleaseDate;
            //result.GaleryUrls = files.Where(f => f.FileCategory == (int)FILE_CATEGORIES.IMAGE).Select(f => f.URL).ToList();
            result.VideoUrls = files.Where(f => f.FileCategory == (int)FILE_CATEGORIES.VIDEO).Select(f => f.URL).ToList();
            result.SocialLinks = files.Where(f => f.FileCategory == (int)FILE_CATEGORIES.SOCIAL_LINKS).Select(f => f.URL).FirstOrDefault();

            var places = await _repository.GetPlaces(request.Id);
            if (places.Count > 0)
            {
                result.Parking = places;
            }
            var comment = await _repository.GetLastRejectedComment(request.Id);
            if (comment != null)
                result.Comment = comment.Text;
            
            return result;
        }
    }
}
