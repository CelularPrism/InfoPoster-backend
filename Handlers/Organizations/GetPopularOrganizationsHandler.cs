using InfoPoster_backend.Models.Administration;
using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Selectel_API;
using InfoPoster_backend.Tools;
using MediatR;

namespace InfoPoster_backend.Handlers.Organizations
{
    public class GetPopularOrganizationsRequest : IRequest<List<OrganizationResponseModel>>
    {
        public POPULARITY_PLACE Place { get; set; }
        public Guid? CategoryId { get; set; }
    }

    public class GetPopularOrganizationsHandler : IRequestHandler<GetPopularOrganizationsRequest, List<OrganizationResponseModel>>
    {
        private readonly OrganizationRepository _repository;
        private readonly FileRepository _file;
        private readonly SelectelAuthService _selectel;
        private readonly Guid _city;

        public GetPopularOrganizationsHandler(OrganizationRepository repository, FileRepository file, SelectelAuthService selectel, IHttpContextAccessor accessor)
        {
            _repository = repository;
            _file = file;
            _selectel = selectel;
            _city = Guid.TryParse(accessor.HttpContext.Request.Headers["X-Testing"].ToString(), out _city) ? Guid.Parse(accessor.HttpContext.Request.Headers["X-Testing"].ToString()) : Constants.DefaultCity;
        }

        public async Task<List<OrganizationResponseModel>> Handle(GetPopularOrganizationsRequest request, CancellationToken cancellationToken = default)
        {
            var result = new List<OrganizationResponseModel>();
            //if (request.CategoryId != null && request.CategoryId != Guid.Empty)
            //{
            //    result = await _repository.GetPopularOrganizationListByCategory(request.Place, (Guid)request.CategoryId);
            //} else
            //{
                result = await _repository.GetPopularOrganizationList(request.Place, _city, request.CategoryId);
            //}

            result = result.Where(o => o.Status == (int)POSTER_STATUS.PUBLISHED).ToList();

            var isLoggedIn = await _selectel.Login();

            if (isLoggedIn)
            {
                var selectelUUID = await _selectel.GetContainerUUID("dosdoc");
                foreach (var item in result)
                {
                    var file = await _file.GetPrimaryFile(item.Id, 0);
                    if (file == null)
                        file = await _file.GetApplicationFileByApplication(item.Id);

                    item.FileURL = string.Concat("https://", selectelUUID, ".selstorage.ru/", file.FileId);
                }
            }

            return result;
        }
    }
}
