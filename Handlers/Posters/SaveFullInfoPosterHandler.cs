using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Cities;
using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Models.Selectel;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Selectel_API;
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
        public Guid City { get; set; }
        public string TimeStart { get; set; }
        public double Price { get; set; }
        public string Adress { get; set; }
        public string PlaceLink { get; set; }
        public List<PlaceRequestModel> Parking { get; set; }
        public string Tags { get; set; }
        public string SocialLinks { get; set; }
        public string Phone { get; set; }
        public string SiteLink { get; set; }
        public string AgeRestriction { get; set; }
        public List<string> VideoUrls { get; set; }
        public string FirstName { get; set; }
        public string ContactPhone { get; set; }
        public string Zalo { get; set; }
        public string Email { get; set; }
        public string ContactDescription { get; set; }
    }

    public class SaveFullInfoPosterResponse
    {
        public Guid Id { get; set; }
    }

    public class SaveFullInfoPosterHandler : IRequestHandler<SaveFullInfoPosterRequest, SaveFullInfoPosterResponse>
    {
        private readonly PosterRepository _repository;
        private readonly SelectelAuthService _selectelAuthService;
        public SaveFullInfoPosterHandler(PosterRepository repository, SelectelAuthService selectelAuthService)
        {
            _repository = repository;
            _selectelAuthService = selectelAuthService;
        }

        public async Task<SaveFullInfoPosterResponse> Handle(SaveFullInfoPosterRequest request, CancellationToken cancellationToken = default)
        {
            var poster = await _repository.GetPoster(request.PosterId);
            if (poster == null || poster.Status == (int)POSTER_STATUS.ACTIVE || poster.Status == (int)POSTER_STATUS.VERIFIED)
                return null;

            var fullInfo = await _repository.GetFullInfoPoster(request.PosterId);
            if (fullInfo == null)
            {
                fullInfo = new PosterFullInfoModel()
                {
                    PosterId = request.PosterId,
                    AgeRestriction = request.AgeRestriction,
                    CategoryId = request.CategoryId,
                    PlaceLink = request.PlaceLink,
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
                fullInfo.PlaceLink = request.PlaceLink;
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
                contact = new ContactModel()
                {
                    ApplicationId = request.PosterId
                };
                contact.Update(request);
                await _repository.AddContact(contact);
            } else
            {
                contact.Update(request);
                await _repository.UpdateContact(contact);
            }

            var files = new List<FileURLModel>();

            if (request.VideoUrls != null)
            {
                foreach (var video in request.VideoUrls)
                {
                    files.Add(new FileURLModel(request.PosterId, video, (int)FILE_CATEGORIES.VIDEO));
                }
            }
            if (!string.IsNullOrEmpty(request.SocialLinks))
            {
                var links = request.SocialLinks.Split(' ');
                foreach (var social in links)
                {
                    files.Add(new FileURLModel(request.PosterId, social, (int)FILE_CATEGORIES.SOCIAL_LINKS));
                }
            }

            await _repository.SaveFiles(files, request.PosterId);

            if (request.Parking != null && request.Parking.Count > 0)
            {
                request.Parking = request.Parking.Select(p => new PlaceRequestModel()
                {
                    Info = p.Info,
                    Lang = request.Lang,
                    PlaceLink = p.PlaceLink,
                }).ToList();
                var places = request.Parking.Select(p => new PlaceModel(p, request.PosterId)).ToList();
                await _repository.SavePlaces(places, request.PosterId);
            }

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

        private async Task SaveBase64(string base64, string type, Guid posterId)
        {
            var loggedIn = await _selectelAuthService.Login();
            if (loggedIn)
            {

                var file = new SelectelFileURLModel()
                {
                    Type = type
                };
                
                var app = new FileToApplication()
                {
                    ApplicationId = posterId,
                    FileId = file.Id
                };

                await _repository.AddSelectelFile(file);
                await _repository.AddFilePoster(app);
                await _selectelAuthService.UploadObject(Convert.FromBase64String(base64), file.Id);
            }
        }
    }
}
