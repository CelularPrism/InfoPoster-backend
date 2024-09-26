using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Posters
{
    public class UpdateFullInfoPosterRequest : IRequest<UpdateFullInfoPosterResponse>
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

    public class UpdateFullInfoPosterResponse
    {
        public Guid Id { get; set; }
    }

    public class UpdateFullInfoPosterHandler : IRequestHandler<UpdateFullInfoPosterRequest, UpdateFullInfoPosterResponse>
    {
        private readonly PosterRepository _repository;
        public UpdateFullInfoPosterHandler(PosterRepository repository)
        {
            _repository = repository;
        }

        public async Task<UpdateFullInfoPosterResponse> Handle(UpdateFullInfoPosterRequest request, CancellationToken cancellationToken = default)
        {
            var poster = await _repository.GetPoster(request.PosterId);
            if (poster == null)
                return null;

            var fullInfo = await _repository.GetFullInfoPoster(request.PosterId);
            fullInfo.PosterId = request.PosterId;
            fullInfo.AgeRestriction = request.AgeRestriction;
            fullInfo.CategoryId = request.CategoryId;
            fullInfo.Latitude = request.latitude;
            fullInfo.Longitude = request.longitude;
            fullInfo.Price = request.Price;
            fullInfo.TimeStart = request.TimeStart;

            var multilang = await _repository.GetMultilangPoster(request.PosterId, request.Lang);
            multilang.Update(request);
            if (string.IsNullOrEmpty(poster.Name))
                poster.Name = request.Name;

            poster.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdatePoster(poster);
            await _repository.UpdatePosterFullInfo(fullInfo);
            await _repository.UpdatePosterMultilang(multilang);

            var result = new UpdateFullInfoPosterResponse()
            {
                Id = poster.Id
            };

            return result;
        }
    }
}
