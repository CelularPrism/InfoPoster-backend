using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Organizations;
using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Login;
using InfoPoster_backend.Tools;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace InfoPoster_backend.Handlers.Organizations
{
    public class ChangeOrganizationStatusRequest : IRequest<ChangeOrganizationStatusResponse>
    {
        public Guid Id { get; set; }
        public POSTER_STATUS Status { get; set; }
    }

    public class ChangeOrganizationStatusResponse
    {
        public bool IsSuccess { get; set; } = true;
        public HttpStatusCode StatusCode { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class ChangeOrganizationStatusHandler : IRequestHandler<ChangeOrganizationStatusRequest, ChangeOrganizationStatusResponse>
    {
        private readonly OrganizationRepository _repository;
        private readonly Guid _user;

        public ChangeOrganizationStatusHandler(OrganizationRepository repository, LoginService loginService)
        {
            _repository = repository;
            _user = loginService.GetUserId();
        }

        public async Task<ChangeOrganizationStatusResponse> Handle(ChangeOrganizationStatusRequest request, CancellationToken cancellationToken = default)
        {
            var organization = await _repository.GetOrganization(request.Id);
            if (organization == null)
                return new ChangeOrganizationStatusResponse() 
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.NotFound,
                    ErrorMessage = "Organization not found"
                };

            if (request.Status == POSTER_STATUS.PUBLISHED)
            {
                var fullInfo = await _repository.GetOrganizationFullInfo(request.Id);

                if (organization.CategoryId == Guid.Empty || organization.SubcategoryId == Guid.Empty || fullInfo.City == null || fullInfo.City == Guid.Empty || string.IsNullOrEmpty(fullInfo.Capacity) || string.IsNullOrEmpty(fullInfo.AgeRestriction) || string.IsNullOrEmpty(fullInfo.PriceLevel))
                {
                    return new ChangeOrganizationStatusResponse()
                    {
                        IsSuccess = false,
                        StatusCode = HttpStatusCode.NotFound,
                        ErrorMessage = "Required fields is empty"
                    };
                }

                OrganizationMultilangModel ml = null;
                foreach (var lang in Constants.SystemLangs)
                {
                    ml = await _repository.GetOrganizationMultilang(request.Id, lang);
                    if (ml == null)
                    {
                        return new ChangeOrganizationStatusResponse()
                        {
                            IsSuccess = false,
                            StatusCode = HttpStatusCode.NotFound,
                            ErrorMessage = "Poster on lang '" + lang + "' not found"
                        };
                    }

                    if (string.IsNullOrEmpty(ml.Name) || string.IsNullOrEmpty(ml.Description))
                    {
                        return new ChangeOrganizationStatusResponse()
                        {
                            IsSuccess = false,
                            StatusCode = HttpStatusCode.NotFound,
                            ErrorMessage = "Required fields is empty on lang '" + lang + "'"
                        };
                    }
                }
            }

            var articleId = Guid.NewGuid();
            var changeHistory = new List<ApplicationChangeHistory>() { new ApplicationChangeHistory(articleId, request.Id, "Status", organization.Status.ToString(), request.Status.ToString(), _user) };
            await _repository.AddHistory(changeHistory);

            organization.Status = (int)request.Status;
            await _repository.UpdateOrganization(organization, _user, articleId);
            return new ChangeOrganizationStatusResponse();
        }
    }
}
