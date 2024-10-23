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
        public string Lang { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        [JsonConverter(typeof(OnlyDateConverter))]
        public DateTime ReleaseDate { get; set; }
        public Guid CategoryId { get; set; }
        public string Place { get; set; } = string.Empty;
        public Guid? City { get; set; }
        public string TimeStart { get; set; } = string.Empty;
        public double Price { get; set; }
        public string Adress { get; set; } = string.Empty;
        public string PlaceLink { get; set; } = string.Empty;
        public List<PlaceModel> Parking { get; set; } = new List<PlaceModel>();
        public string Tags { get; set; } = string.Empty;
        public List<string> SocialLinks { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string SiteLink { get; set; } = string.Empty;
        public string AgeRestriction { get; set; } = string.Empty;
        public List<string> VideoUrls { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string Zalo { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ContactDescription { get; set; } = string.Empty;
        public int Status { get; set; }
    }

    public class AdministrationGetPosterByIdHandler : IRequestHandler<AdministrationGetPosterByIdRequest, AdministrationGetPosterByIdResponse>
    {
        private readonly PosterRepository _repository;

        public AdministrationGetPosterByIdHandler(PosterRepository repository)
        {
            _repository = repository;
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
                result.Phone = !string.IsNullOrEmpty(ml.Phone) ? ml.Phone : string.Empty;
                result.Place = !string.IsNullOrEmpty(ml.Place) ? ml.Place : string.Empty;
                result.SiteLink = !string.IsNullOrEmpty(ml.SiteLink) ? ml.SiteLink : string.Empty;
            } else
            {
                result.Lang = request.Lang;
            }

            var contact = await _repository.GetContact(request.Id);
            if (contact != null)
            {
                result.FirstName = !string.IsNullOrEmpty(contact.Name) ? contact.Name : string.Empty;
                result.ContactPhone = !string.IsNullOrEmpty(contact.Phone) ? contact.Phone : string.Empty;
                result.Zalo = !string.IsNullOrEmpty(contact.Zalo) ? contact.Zalo : string.Empty;
                result.Email = !string.IsNullOrEmpty(contact.Email) ? contact.Email : string.Empty;
                result.ContactDescription = !string.IsNullOrEmpty(contact.Comment) ? contact.Comment : string.Empty;
            }
            
            var files = await _repository.GetFileUrls(request.Id);

            result.ReleaseDate = poster.ReleaseDate;
            //result.GaleryUrls = files.Where(f => f.FileCategory == (int)FILE_CATEGORIES.IMAGE).Select(f => f.URL).ToList();
            result.VideoUrls = files.Where(f => f.FileCategory == (int)FILE_CATEGORIES.VIDEO).Select(f => f.URL).ToList();
            result.SocialLinks = files.Where(f => f.FileCategory == (int)FILE_CATEGORIES.SOCIAL_LINKS).Select(f => f.URL).ToList();

            var places = await _repository.GetPlaces(request.Id);
            if (places.Count > 0)
            {
                result.Parking = places;
            }
            
            return result;
        }
    }
}
