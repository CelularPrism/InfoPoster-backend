using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using MediatR;

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
        public DateTime ReleaseDate { get; set; }
        public Guid CategoryId { get; set; }
        public string Place { get; set; }
        public string City { get; set; }
        public string TimeStart { get; set; }
        public double Price { get; set; }
        public string Adress { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string Parking { get; set; }
        public string ParkingPlace { get; set; }
        public string Tags { get; set; }
        public string SocialLinks { get; set; }
        public string Phone { get; set; }
        public string SiteLink { get; set; }
        public string AgeRestriction { get; set; }
        public List<string> GaleryUrls { get; set; }
        public List<string> VideoUrls { get; set; }
        public string FirstName { get; set; }
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

            var fullInfo = await _repository.GetFullInfoPoster(request.Id);
            if (fullInfo != null)
            {
                result.AgeRestriction = fullInfo.AgeRestriction;
                result.CategoryId = fullInfo.CategoryId;
                result.latitude = fullInfo.Latitude;
                result.longitude = fullInfo.Longitude;
                result.TimeStart = fullInfo.TimeStart;
                result.PosterId = fullInfo.PosterId;
                result.Price = fullInfo.Price;
            } else
            {
                result.PosterId = poster.Id;
            }

            var ml = await _repository.GetMultilangPoster(request.Id, request.Lang);
            if (ml != null)
            {
                result.Adress = ml.Adress;
                result.City = ml.City;
                result.Lang = ml.Lang;
                result.Description = ml.Description;
                result.Name = ml.Name;
                result.Parking = ml.Parking;
                result.ParkingPlace = ml.ParkingPlace;
                result.Phone = ml.Phone;
                result.Place = ml.Place;
                result.SiteLink = ml.SiteLink;
                result.SocialLinks = ml.SocialLinks;
            } else
            {
                result.Lang = request.Lang;
            }

            var contact = await _repository.GetContact(request.Id);
            if (contact != null)
            {
                result.FirstName = contact.FirstName;
            }
            
            var files = await _repository.GetFileUrls(request.Id);

            result.ReleaseDate = poster.ReleaseDate;
            result.GaleryUrls = files.Where(f => f.FileCategory == (int)FILE_CATEGORIES.IMAGE).Select(f => f.URL).ToList();
            result.VideoUrls = files.Where(f => f.FileCategory == (int)FILE_CATEGORIES.VIDEO).Select(f => f.URL).ToList();

            return result;
        }
    }
}
