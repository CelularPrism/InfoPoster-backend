using InfoPoster_backend.Handlers.Administration;
using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Cities;
using InfoPoster_backend.Models.Organizations;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Selectel_API;
using MediatR;

namespace InfoPoster_backend.Handlers.Organizations
{
    public class GetFullInfoOrganizationRequest : IRequest<GetFullInfoOrganizationResponse>
    {
        public Guid Id { get; set; }
    }

    public class GetFullInfoOrganizationResponse
    {
        public GetFullInfoOrganizationResponse() { }

        public GetFullInfoOrganizationResponse(OrganizationModel organization, OrganizationFullInfoModel fullInfo, OrganizationMultilangModel multilang)
        {
            Id = organization.Id;
            CategoryId = organization.CategoryId;
            SubcategoryId = organization.SubcategoryId;
            CityId = fullInfo.City;
            Name = multilang.Name;
            PriceLevel = fullInfo.PriceLevel;
            Capacity = fullInfo.Capacity;
            WorkTime = multilang.WorkTime;
            Adress = multilang.Adress;
            PlaceLink = fullInfo.PlaceLink;
            SiteLink = multilang.SiteLink;
            AgeRestriction = fullInfo.AgeRestriction;
            Description = multilang.Description;
            Phone = multilang.Phone;
        }

        public Guid Id { get; set; }
        public string Lang { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }
        public Guid SubcategoryId { get; set; }
        public string SubcategoryName { get; set; }
        public string PriceLevel { get; set; } = string.Empty;
        public string Capacity { get; set; } = string.Empty;
        public Guid? CityId { get; set; }
        public string City { get; set; }
        public string WorkTime { get; set; } = string.Empty;
        public string Adress { get; set; } = string.Empty;
        public string PlaceLink { get; set; } = string.Empty;
        public string SiteLink { get; set; } = string.Empty;
        public string AgeRestriction { get; set; } = string.Empty;
        public string SocialLinks { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Contacts { get; set; } = string.Empty;
        public List<PlaceModel> Parking { get; set; } = new List<PlaceModel>();
        public string Phone { get; set; } = string.Empty;
        public List<GetFileResponse> GaleryUrls { get; set; }
        public List<GetFileResponse> MenuUrls { get; set; }
        public List<string> VideoUrls { get; set; }
        public List<string> MenuCategories { get; set; }
    }

    public class GetFullInfoOrganizationHandler : IRequestHandler<GetFullInfoOrganizationRequest, GetFullInfoOrganizationResponse>
    {
        private readonly OrganizationRepository _repository;
        private readonly FileRepository _fileRepository;
        private readonly SelectelAuthService _selectelAuthService;

        public GetFullInfoOrganizationHandler(OrganizationRepository repository, FileRepository fileRepository, SelectelAuthService selectelAuthService)
        {
            _repository = repository;
            _fileRepository = fileRepository;
            _selectelAuthService = selectelAuthService;
        }

        public async Task<GetFullInfoOrganizationResponse> Handle(GetFullInfoOrganizationRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _repository.GetFullInfo(request.Id);
            var files = await _fileRepository.GetSelectelFiles(request.Id, (int)FILE_PLACES.GALLERY);
            var primaryFile = await _fileRepository.GetPrimaryFile(request.Id, (int)FILE_PLACES.GALLERY);
            var menu = await _fileRepository.GetSelectelFiles(request.Id, (int)FILE_PLACES.ORGANIZATION_MENU);
            var primaryMenu = await _fileRepository.GetPrimaryFile(request.Id, (int)FILE_PLACES.ORGANIZATION_MENU);

            var loggedIn = await _selectelAuthService.Login();

            result.GaleryUrls = new List<GetFileResponse>();
            result.MenuUrls = new List<GetFileResponse>();
            if (loggedIn)
            {
                var selectelUUID = await _selectelAuthService.GetContainerUUID("dosdoc");
                var imageSrc = string.Empty;
                GetFileResponse response = null;
                foreach (var file in files)
                {
                    imageSrc = string.Concat("https://", selectelUUID, ".selstorage.ru/", file.Id);
                    response = new GetFileResponse()
                    {
                        Id = file.Id,
                        Type = file.Type,
                        URL = imageSrc,
                        IsPrimary = primaryFile != null && primaryFile.FileId == file.Id ? true : false,
                    };
                    result.GaleryUrls.Add(response);
                }
                result.GaleryUrls = result.GaleryUrls.OrderByDescending(f => f.IsPrimary).ToList();

                foreach (var file in menu)
                {
                    imageSrc = string.Concat("https://", selectelUUID, ".selstorage.ru/", file.Id);
                    response = new GetFileResponse()
                    {
                        Id = file.Id,
                        Type = file.Type,
                        URL = imageSrc,
                        IsPrimary = primaryMenu != null && primaryMenu.FileId == file.Id ? true : false,
                    };
                    result.MenuUrls.Add(response);
                }
                result.MenuUrls = result.MenuUrls.OrderByDescending(f => f.IsPrimary).ToList();
            }

            await _repository.AddViewLog(new Models.Posters.PosterViewLogModel()
            {
                Date = DateTime.UtcNow,
                Id = Guid.NewGuid(),
                PosterId = result.Id
            });

            return result;
        }
    }
}
