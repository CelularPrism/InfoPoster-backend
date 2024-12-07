using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Cities;
using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Models.Selectel;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Login;
using InfoPoster_backend.Services.Selectel_API;
using InfoPoster_backend.Tools;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace InfoPoster_backend.Handlers.Posters
{
    public class SaveFullInfoPosterRequest : IRequest<SaveFullInfoPosterResponse>
    {
        public Guid PosterId { get; set; }
        public string Lang { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ReleaseDate { get; set; }
        public Guid? CategoryId { get; set; }
        public string Place { get; set; }
        public Guid? City { get; set; }
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
        public Guid? AttachedOrganizationId { get; set; }
        public string Tickets { get; set; }
        public string Contacts { get; set; }
        public string InternalContacts { get; set; }
    }

    public class SaveFullInfoPosterResponse
    {
        public Guid Id { get; set; }
    }

    public class SaveFullInfoPosterHandler : IRequestHandler<SaveFullInfoPosterRequest, SaveFullInfoPosterResponse>
    {
        private readonly PosterRepository _repository;
        private readonly SelectelAuthService _selectelAuthService;
        private readonly Guid _user;

        public SaveFullInfoPosterHandler(PosterRepository repository, SelectelAuthService selectelAuthService, LoginService loginService)
        {
            _repository = repository;
            _selectelAuthService = selectelAuthService;
            _user = loginService.GetUserId();
        }

        public async Task<SaveFullInfoPosterResponse> Handle(SaveFullInfoPosterRequest request, CancellationToken cancellationToken = default)
        {
            var poster = await _repository.GetPoster(request.PosterId);
            var isAdmin = await _repository.CheckAdmin(_user);
            if (poster == null || ((poster.Status == (int)POSTER_STATUS.PENDING || poster.Status == (int)POSTER_STATUS.PUBLISHED) && !isAdmin))
                return null;

            var changeHistory = new List<ApplicationChangeHistory>();
            var history = new List<ApplicationChangeHistory>();
            var articleId = Guid.NewGuid();

            var fullInfo = await _repository.GetFullInfoPoster(request.PosterId);
            if (fullInfo == null)
            {
                fullInfo = new PosterFullInfoModel()
                {
                    PosterId = request.PosterId
                };

                history = fullInfo.Update(request, articleId, _user);
                changeHistory.AddRange(history);
                await _repository.AddPosterFullInfo(fullInfo);
            }
            else 
            { 
                fullInfo.PosterId = request.PosterId;
                history = fullInfo.Update(request, articleId, _user);
                changeHistory.AddRange(history);

                await _repository.UpdatePosterFullInfo(fullInfo);
            }

            var multilang = await _repository.GetMultilangPosterList(request.PosterId);
            if (multilang == null || multilang.Count == 0)
            {
                foreach (var lang in Constants.SystemLangs) 
                {
                    var ml = new PosterMultilangModel();
                    ml.Id = Guid.NewGuid();
                    ml.PosterId = request.PosterId;
                    ml.Lang = lang;
                    history = ml.Update(request, articleId, _user);
                    changeHistory.AddRange(history);

                    multilang.Add(ml);
                }
                await _repository.AddPosterMultilang(multilang);
            }
            else
            {
                try
                {
                    foreach (var ml in multilang)
                    {
                        history = ml.Update(request, articleId, _user);
                        changeHistory.AddRange(history);
                    }
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
                var contactList = new List<ContactModel>();
                foreach (var lang in Constants.SystemLangs)
                {
                    contact = new ContactModel()
                    {
                        ApplicationId = request.PosterId,
                        Lang = lang
                    };
                    history = contact.Update(request, articleId, _user);
                    changeHistory.AddRange(history);
                    contactList.Add(contact);
                }
                await _repository.AddContact(contactList, request.PosterId);
            } else
            {
                history = contact.Update(request, articleId, _user);
                changeHistory.AddRange(history);
                await _repository.UpdateContact(contact);
            }

            var files = new List<FileURLModel>();
            var filesOld = await _repository.GetFileUrls(request.PosterId);

            if (request.VideoUrls != null)
            {
                changeHistory.Add(new ApplicationChangeHistory(articleId, request.PosterId, "VideoUrls",
                    "Count = " + filesOld.Where(f => f.FileCategory == (int)FILE_CATEGORIES.VIDEO).Count(),
                    "Count = " + request.VideoUrls.Count, _user));

                foreach (var video in request.VideoUrls)
                {
                    files.Add(new FileURLModel(request.PosterId, video, (int)FILE_CATEGORIES.VIDEO));
                }
            }
            if (!string.IsNullOrEmpty(request.SocialLinks))
            {
                var links = request.SocialLinks;
                changeHistory.Add(new ApplicationChangeHistory(articleId, request.PosterId, "SocialLinks", string.Empty, string.Empty, _user));
                files.Add(new FileURLModel(request.PosterId, links, (int)FILE_CATEGORIES.SOCIAL_LINKS));
            }

            await _repository.SaveFiles(files, request.PosterId);

            if (request.Parking != null && request.Parking.Count > 0)
            {
                var places = await _repository.GetPlaceList(request.PosterId);
                if (places != null || places.Count > 0)
                {
                    await _repository.RemovePlaceList(places);
                }
                places = new List<PlaceModel>();
                foreach (var lang in Constants.SystemLangs)
                {
                    request.Parking = request.Parking.Select(p => new PlaceRequestModel()
                    {
                        Info = p.Info,
                        Lang = lang,
                        PlaceLink = p.PlaceLink,
                    }).ToList();
                    places.AddRange(request.Parking.Select(p => new PlaceModel(p, request.PosterId)).ToList());
                }
                await _repository.AddPlaces(places);
                changeHistory.Add(new ApplicationChangeHistory(articleId, request.PosterId, "Parking", string.Empty, string.Empty, _user));
            }

            if (string.IsNullOrEmpty(poster.Name))
                poster.Name = request.Name;

            poster.ReleaseDate = request.ReleaseDate.HasValue ? request.ReleaseDate.Value.Date : null;

            var categories = await _repository.GetCategories();

            if (poster.CategoryId != request.CategoryId)
            {
                changeHistory.Add(new ApplicationChangeHistory(articleId, request.PosterId, "CategoryId",
                    categories.Where(c => c.Id == poster.CategoryId).Select(c => c.Name).FirstOrDefault(),
                    categories.Where(c => c.Id == request.CategoryId).Select(c => c.Name).FirstOrDefault(), _user));

                poster.CategoryId = request.CategoryId == null ? Guid.Empty : (Guid)request.CategoryId;
            }

            
            poster.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdatePoster(poster, _user);

            var result = new SaveFullInfoPosterResponse()
            {
                Id = poster.Id
            };

            return result;
        }
    }
}
