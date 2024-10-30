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

namespace InfoPoster_backend.Handlers.Organizations
{
    public class SaveOrganizationRequest : IRequest<SaveOrganizationResponse>
    {
        public Guid OrganizationId { get; set; }
        public string Lang { get; set; }
        public string Name { get; set; }
        public Guid CategoryId { get; set; }
        public Guid SubcategoryId { get; set; }
        public string PriceLevel { get; set; }
        public string Capacity { get; set; }
        public Guid City { get; set; }
        public string WorkTime { get; set; }
        public string Adress { get; set; }
        public string PlaceLink { get; set; }
        public string SiteLink { get; set; }
        public string AgeRestriction { get; set; }
        public string SocialLinks { get; set; }
        public string Description { get; set; }
        public List<PlaceRequestModel> ParkingOrg { get; set; }
        public string Phone { get; set; }
        public string ContactName { get; set; }
        public List<string> VideoUrls { get; set; }
        public string FirstName { get; set; }
        public string ContactPhone { get; set; }
        public string Zalo { get; set; }
        public string Email { get; set; }
        public string ContactDescription { get; set; }
        public List<Guid> MenuCategories { get; set; }
    }

    public class SaveOrganizationResponse
    {

    }

    public class SaveOrganizationHandler : IRequestHandler<SaveOrganizationRequest, SaveOrganizationResponse>
    {
        private readonly LoginService _loginService;
        private readonly OrganizationRepository _repository;
        private readonly SelectelAuthService _selectelAuthService;

        public SaveOrganizationHandler(LoginService loginService, OrganizationRepository repository, SelectelAuthService selectelAuthService)
        {
            _loginService = loginService;
            _repository = repository;
            _selectelAuthService = selectelAuthService;
        }

        public async Task<SaveOrganizationResponse> Handle(SaveOrganizationRequest request, CancellationToken cancellationToken = default)
        {
            var organization = await _repository.GetOrganization(request.OrganizationId);
            if (organization == null || organization.Status == (int)POSTER_STATUS.ACTIVE || organization.Status == (int)POSTER_STATUS.VERIFIED)
                return null;

            var fullInfo = await _repository.GetOrganizationFullInfo(request.OrganizationId);
            if (fullInfo == null)
            {
                fullInfo = new OrganizationFullInfoModel(request);
                await _repository.AddFullInfo(fullInfo);
            } else
            {
                fullInfo.Update(request);
                await _repository.UpdateFullInfo(fullInfo);
            }

            var ml = await _repository.GetOrganizationMultilangList(request.OrganizationId);
            if (ml == null || ml.Count == 0)
            {
                ml = new List<OrganizationMultilangModel>();
                foreach (var lang in Constants.SystemLangs)
                {
                    ml.Add(new OrganizationMultilangModel(request, lang));
                }
                await _repository.AddMultilang(ml);
            } else
            {
                foreach (var multilang in ml)
                {
                    multilang.Update(request);
                }
                await _repository.UpdateMultilang(ml);
            }

            var contact = await _repository.GetContact(request.OrganizationId);
            if (contact == null)
            {
                contact = new ContactModel()
                {
                    ApplicationId = request.OrganizationId
                };
                contact.Update(request);
                await _repository.AddContact(contact);
            } else
            {
                contact.Update(request);
                await _repository.UpdateContact(contact);
            }

            var files = new List<OrganizationFileURLModel>();
            if (request.VideoUrls != null)
            {
                foreach (var video in request.VideoUrls)
                {
                    files.Add(new OrganizationFileURLModel(request.OrganizationId, video, (int)FILE_CATEGORIES.VIDEO));
                }
            }
            if (!string.IsNullOrEmpty(request.SocialLinks))
            {
                var links = request.SocialLinks.Split(' ');
                foreach (var social in links)
                {
                    files.Add(new OrganizationFileURLModel(request.OrganizationId, social, (int)FILE_CATEGORIES.SOCIAL_LINKS));
                }
            }

            var menus = request.MenuCategories.Select(m => new MenuToOrganizationModel()
            {
                Id = Guid.NewGuid(),
                MenuId = m,
                OrganizationId = request.OrganizationId
            }).ToList();

            await _repository.SaveFiles(files, request.OrganizationId);
            await _repository.SaveMenus(menus, request.OrganizationId);

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
                await _repository.AddPlaces(places);               
            }

            if (string.IsNullOrEmpty(organization.Name))
                organization.Name = request.Name;

            organization.CategoryId = request.CategoryId;
            organization.SubcategoryId = request.SubcategoryId;

            await _repository.UpdateOrganization(organization);

            return new SaveOrganizationResponse();
        }

        private async Task SaveBase64(string base64, string type, Guid organizationId)
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
                    ApplicationId = organizationId,
                    FileId = file.Id
                };

                await _repository.AddSelectelFile(file);
                await _repository.AddFilePoster(app);
                await _selectelAuthService.UploadObject(Convert.FromBase64String(base64), file.Id);
            }
        }
    }
}
