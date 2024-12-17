using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Cities;
using InfoPoster_backend.Models.Organizations;
using InfoPoster_backend.Models.Organizations.Menu;
using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Models.Selectel;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Login;
using InfoPoster_backend.Services.Selectel_API;
using InfoPoster_backend.Tools;
using MediatR;
using Org.BouncyCastle.Utilities.Collections;

namespace InfoPoster_backend.Handlers.Organizations
{
    public class SaveOrganizationRequest : IRequest<SaveOrganizationResponse>
    {
        public Guid OrganizationId { get; set; }
        public string Lang { get; set; }
        public string Name { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? SubcategoryId { get; set; }
        public string PriceLevel { get; set; }
        public string Capacity { get; set; }
        public Guid? City { get; set; }
        public string WorkTime { get; set; }
        public string Adress { get; set; }
        public string PlaceLink { get; set; }
        public string SiteLink { get; set; }
        public string AgeRestriction { get; set; }
        public string SocialLinks { get; set; }
        public string Description { get; set; }
        public List<PlaceRequestModel> ParkingOrg { get; set; }
        public string Contacts { get; set; }
        public string InternalContacts { get; set; }
        public List<string> VideoUrls { get; set; }
        public List<Guid> MenuCategories { get; set; }
    }

    public class SaveOrganizationResponse
    {

    }

    public class SaveOrganizationHandler : IRequestHandler<SaveOrganizationRequest, SaveOrganizationResponse>
    {
        private readonly OrganizationRepository _repository;
        private readonly Guid _user;

        public SaveOrganizationHandler(LoginService loginService, OrganizationRepository repository)
        {
            _user = loginService.GetUserId();
            _repository = repository;
        }

        public async Task<SaveOrganizationResponse> Handle(SaveOrganizationRequest request, CancellationToken cancellationToken = default)
        {
            var organization = await _repository.GetOrganization(request.OrganizationId);
            var isAdmin = await _repository.CheckAdmin(_user);
            if (organization == null || ((organization.Status == (int)POSTER_STATUS.PENDING || organization.Status == (int)POSTER_STATUS.PUBLISHED || organization.Status == (int)POSTER_STATUS.REVIEWING) && !isAdmin))
                return null;

            var articleId = Guid.NewGuid();
            var changeHistory = new List<ApplicationChangeHistory>();

            var fullInfo = await _repository.GetOrganizationFullInfo(request.OrganizationId);
            if (fullInfo == null)
            {
                fullInfo = new OrganizationFullInfoModel();
                fullInfo.OrganizationId = request.OrganizationId;

                var history = fullInfo.Update(request, articleId, _user);
                changeHistory.AddRange(history);
                await _repository.AddFullInfo(fullInfo);
            } else
            {
                var history = fullInfo.Update(request, articleId, _user);
                changeHistory.AddRange(history);
                await _repository.UpdateFullInfo(fullInfo);
            }

            var ml = await _repository.GetOrganizationMultilangList(request.OrganizationId);
            if (ml == null || ml.Count == 0)
            {
                ml = new List<OrganizationMultilangModel>();
                foreach (var lang in Constants.SystemLangs)
                {
                    var mlModel = new OrganizationMultilangModel();
                    mlModel.Lang = lang;
                    mlModel.OrganizationId = request.OrganizationId;
                    var history = mlModel.Update(request, articleId, _user);
                    changeHistory.AddRange(history);

                    ml.Add(mlModel);
                }
                await _repository.AddMultilang(ml);
            } else
            {
                foreach (var multilang in ml)
                {
                    var history = multilang.Update(request, articleId, _user);
                    changeHistory.AddRange(history);
                }
                await _repository.UpdateMultilang(ml);
            }

            var contact = await _repository.GetContact(request.OrganizationId);
            if (contact == null)
            {
                var contactList = new List<ContactModel>();
                foreach (var lang in Constants.SystemLangs)
                {
                    contact = new ContactModel()
                    {
                        ApplicationId = request.OrganizationId,
                        Lang = lang,
                    };
                    var history = contact.Update(request, articleId, _user);
                    changeHistory.AddRange(history);

                    contactList.Add(contact);
                }
                await _repository.AddContact(contactList, request.OrganizationId);
            } else
            {
                var history = contact.Update(request, articleId, _user);
                changeHistory.AddRange(history);
                await _repository.UpdateContact(contact);
            }

            var files = new List<OrganizationFileURLModel>();
            var filesOld = await _repository.GetFileUrls(request.OrganizationId);
            if (request.VideoUrls != null)
            {
                changeHistory.Add(new ApplicationChangeHistory(articleId, request.OrganizationId, "VideoUrls", 
                    "Count = " + filesOld.Where(f => f.FileCategory == (int)FILE_CATEGORIES.VIDEO).Count(), 
                    "Count = " + request.VideoUrls.Count, _user));

                foreach (var video in request.VideoUrls)
                {
                    files.Add(new OrganizationFileURLModel(request.OrganizationId, video, (int)FILE_CATEGORIES.VIDEO));
                }
            }
            if (!string.IsNullOrEmpty(request.SocialLinks))
            {
                var links = request.SocialLinks;
                changeHistory.Add(new ApplicationChangeHistory(articleId, request.OrganizationId, "SocialLinks", string.Empty, string.Empty, _user));
                files.Add(new OrganizationFileURLModel(request.OrganizationId, links, (int)FILE_CATEGORIES.SOCIAL_LINKS));
            }

            await _repository.SaveFiles(files, request.OrganizationId);

            if (request.MenuCategories != null)
            {
                changeHistory.Add(new ApplicationChangeHistory(articleId, request.OrganizationId, "MenuCategories", string.Empty, string.Empty, _user));
                var menus = request.MenuCategories.Select(m => new MenuToOrganizationModel()
                {
                    Id = Guid.NewGuid(),
                    MenuId = m,
                    OrganizationId = request.OrganizationId
                }).ToList();
                await _repository.SaveMenus(menus, request.OrganizationId);
            }

            if (request.ParkingOrg != null && request.ParkingOrg.Count > 0)
            {
                var places = await _repository.GetPlaceList(request.OrganizationId);
                if (places != null || places.Count > 0)
                {
                    await _repository.RemovePlaceList(places);
                }
                places = new List<PlaceModel>();
                foreach (var lang in Constants.SystemLangs)
                {
                    request.ParkingOrg = request.ParkingOrg.Select(p => new PlaceRequestModel()
                    {
                        Info = p.Info,
                        Lang = lang,
                        PlaceLink = p.PlaceLink,
                    }).ToList();
                    places.AddRange(request.ParkingOrg.Select(p => new PlaceModel(p, request.OrganizationId)).ToList());
                }
                changeHistory.Add(new ApplicationChangeHistory(articleId, request.OrganizationId, "Parking", string.Empty, string.Empty, _user));
                await _repository.AddPlaces(places);               
            }

            if (string.IsNullOrEmpty(organization.Name))
                organization.Name = request.Name;

            var categories = await _repository.GetCategories();
            var subcategories = await _repository.GetSubcategories();

            if (organization.CategoryId != request.CategoryId)
            {
                changeHistory.Add(new ApplicationChangeHistory(articleId, request.OrganizationId, "CategoryId", 
                    categories.Where(c => c.Id == organization.CategoryId).Select(c => c.Name).FirstOrDefault(), 
                    categories.Where(c => c.Id == request.CategoryId).Select(c => c.Name).FirstOrDefault(), _user));

                organization.CategoryId = request.CategoryId == null ? Guid.Empty : (Guid)request.CategoryId;
            }

            if (organization.SubcategoryId != request.SubcategoryId)
            {
                changeHistory.Add(new ApplicationChangeHistory(articleId, request.OrganizationId, "SubcategoryId",
                    subcategories.Where(c => c.Id == organization.SubcategoryId).Select(c => c.Name).FirstOrDefault(),
                    subcategories.Where(c => c.Id == request.SubcategoryId).Select(c => c.Name).FirstOrDefault(), _user));

                organization.SubcategoryId = request.SubcategoryId == null ? Guid.Empty : (Guid)request.SubcategoryId;
            }

            await _repository.AddHistory(changeHistory);
            await _repository.UpdateOrganization(organization, _user, articleId);

            return new SaveOrganizationResponse();
        }
    }
}
