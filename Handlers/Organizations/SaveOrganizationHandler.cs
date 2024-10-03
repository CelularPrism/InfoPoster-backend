using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Organizations;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Login;
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
        public string City { get; set; }
        public string WorkTime { get; set; }
        public string Adress { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string SiteLink { get; set; }
        public string AgeRestriction { get; set; }
        public string SocialLinks { get; set; }
        public string Description { get; set; }
        public string ParkingInfo { get; set; }
        public string ParkingPlace { get; set; }
        public string Phone { get; set; }
        public string ContactName { get; set; }
        public List<string> GaleryUrls { get; set; }
        public List<string> VideoUrls { get; set; }
        public string FirstName { get; set; }
        public string Zalo { get; set; }
        public string Email { get; set; }
        public string ContactDescription { get; set; }
    }

    public class SaveOrganizationResponse
    {

    }

    public class SaveOrganizationHandler : IRequestHandler<SaveOrganizationRequest, SaveOrganizationResponse>
    {
        private readonly LoginService _loginService;
        private readonly OrganizationRepository _repository;

        public SaveOrganizationHandler(LoginService loginService, OrganizationRepository repository)
        {
            _loginService = loginService;
            _repository = repository;
        }

        public async Task<SaveOrganizationResponse> Handle(SaveOrganizationRequest request, CancellationToken cancellationToken = default)
        {
            var organization = await _repository.GetOrganization(request.OrganizationId);
            if (organization == null)
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

            var ml = await _repository.GetOrganizationMultilang(request.OrganizationId, request.Lang);
            if (ml == null)
            {
                ml = new OrganizationMultilangModel(request);
                await _repository.AddMultilang(ml);
            } else
            {
                ml.Update(request);
                await _repository.UpdateMultilang(ml);
            }

            var files = new List<OrganizationFileURLModel>();
            if (request.GaleryUrls != null)
            {
                foreach (var img in request.GaleryUrls)
                {
                    files.Add(new OrganizationFileURLModel(request.OrganizationId, img, (int)FILE_CATEGORIES.IMAGE));
                }
            }
            if (request.VideoUrls != null)
            {
                foreach (var video in request.VideoUrls)
                {
                    files.Add(new OrganizationFileURLModel(request.OrganizationId, video, (int)FILE_CATEGORIES.VIDEO));
                }
            }

            await _repository.SaveFiles(files, request.OrganizationId);

            return new SaveOrganizationResponse();

        }
    }
}
