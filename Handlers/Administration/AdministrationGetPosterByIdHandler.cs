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
            var fullInfo = await _repository.GetFullInfoPoster(request.Id);
            var ml = await _repository.GetMultilangPoster(request.Id, request.Lang);

            var result = new AdministrationGetPosterByIdResponse()
            {
                Adress = ml.Adress,
                AgeRestriction = fullInfo.AgeRestriction,
                CategoryId = fullInfo.CategoryId,
                City = ml.City,
                TimeStart = fullInfo.TimeStart,
                Lang = ml.Lang,
                Description = ml.Description,
                latitude = fullInfo.Latitude,
                longitude = fullInfo.Longitude,
                Name = ml.Name,
                Parking = ml.Parking,
                ParkingPlace = ml.ParkingPlace,
                Phone = ml.Phone,
                Place = ml.Place,
                PosterId = fullInfo.PosterId,
                Price = fullInfo.Price,
                ReleaseDate = poster.ReleaseDate,
                SiteLink = ml.SiteLink
            };

            return result;
        }
    }
}
