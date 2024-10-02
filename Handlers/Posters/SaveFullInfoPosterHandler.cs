using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Posters
{
    public class SaveFullInfoPosterRequest : IRequest<SaveFullInfoPosterResponse>
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

    public class SaveFullInfoPosterResponse
    {
        public Guid Id { get; set; }
    }

    public class SaveFullInfoPosterHandler : IRequestHandler<SaveFullInfoPosterRequest, SaveFullInfoPosterResponse>
    {
        private readonly PosterRepository _repository;
        public SaveFullInfoPosterHandler(PosterRepository repository)
        {
            _repository = repository;
        }

        public async Task<SaveFullInfoPosterResponse> Handle(SaveFullInfoPosterRequest request, CancellationToken cancellationToken = default)
        {
            var poster = await _repository.GetPoster(request.PosterId);
            if (poster == null)
                return null;

            var fullInfo = await _repository.GetFullInfoPoster(request.PosterId);
            if (fullInfo == null)
            {
                fullInfo = new PosterFullInfoModel()
                {
                    PosterId = request.PosterId,
                    AgeRestriction = request.AgeRestriction,
                    CategoryId = request.CategoryId,
                    Latitude = request.latitude,
                    Longitude = request.longitude,
                    Price = request.Price,
                    TimeStart = request.TimeStart
                };
                await _repository.AddPosterFullInfo(fullInfo);
            }
            else 
            { 
                fullInfo.PosterId = request.PosterId;
                fullInfo.AgeRestriction = request.AgeRestriction;
                fullInfo.CategoryId = request.CategoryId;
                fullInfo.Latitude = request.latitude;
                fullInfo.Longitude = request.longitude;
                fullInfo.Price = request.Price;
                fullInfo.TimeStart = request.TimeStart;
                await _repository.UpdatePosterFullInfo(fullInfo);
            }

            var multilang = await _repository.GetMultilangPoster(request.PosterId, request.Lang);
            if (multilang == null)
            {
                multilang = new PosterMultilangModel(request);
                await _repository.AddPosterMultilang(multilang);
            }
            else
            {
                try
                {
                    multilang.Update(request);
                    await _repository.UpdatePosterMultilang(multilang);
                }
                catch (Exception ex)
                {
                    var newEx = ex;
                }
            }

            var contact = await _repository.GetContact(request.PosterId);

            if (contact == null)
            {
                contact = new PosterContactsModel(fullInfo.PosterId, multilang.Phone, request.FirstName);
                await _repository.AddContact(contact);
            } else
            {
                contact.Update(multilang.Phone, request.FirstName);
                await _repository.UpdateContact(contact);
            }

            var files = new List<FileURLModel>();
            if (request.GaleryUrls != null)
            {
                foreach (var img in request.GaleryUrls)
                {
                    files.Add(new FileURLModel(request.PosterId, img, (int)FILE_CATEGORIES.IMAGE));
                }
            }
            if (request.VideoUrls != null)
            {
                foreach (var video in request.VideoUrls)
                {
                    files.Add(new FileURLModel(request.PosterId, video, (int)FILE_CATEGORIES.VIDEO));
                }
            }

            await _repository.SaveFiles(files, request.PosterId);

            if (string.IsNullOrEmpty(poster.Name))
                poster.Name = request.Name;

            poster.ReleaseDate = request.ReleaseDate;
            poster.CategoryId = request.CategoryId;
            poster.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdatePoster(poster);

            var result = new SaveFullInfoPosterResponse()
            {
                Id = poster.Id
            };

            return result;
        }
    }
}
