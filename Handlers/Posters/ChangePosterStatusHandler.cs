using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Login;
using InfoPoster_backend.Tools;
using MediatR;
using System.Net;

namespace InfoPoster_backend.Handlers.Posters
{
    public class ChangePosterStatusRequest : IRequest<ChangePosterStatusResponse>
    {
        public Guid Id { get; set; }
        public POSTER_STATUS Status { get; set; }
    }

    public class ChangePosterStatusResponse 
    {
        public bool IsSuccess { get; set; } = true;
        public HttpStatusCode StatusCode { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class ChangePosterStatusHandler : IRequestHandler<ChangePosterStatusRequest, ChangePosterStatusResponse>
    {
        private readonly PosterRepository _repository;
        private readonly Guid _user;

        public ChangePosterStatusHandler(PosterRepository repository, LoginService loginService)
        {
            _repository = repository;
            _user = loginService.GetUserId();
        }

        public async Task<ChangePosterStatusResponse> Handle(ChangePosterStatusRequest request, CancellationToken cancellationToken = default)
        {
            var poster = await _repository.GetPoster(request.Id);
            if (poster == null)
                return new ChangePosterStatusResponse()
                {
                    IsSuccess = false,
                    StatusCode = HttpStatusCode.NotFound,
                    ErrorMessage = "Poster not found"
                };

            if (request.Status == POSTER_STATUS.PUBLISHED)
            {
                var fullInfo = await _repository.GetFullInfoPoster(request.Id);
                //if (fullInfo.OrganizationId == null || !await _repository.AnyOrganization((Guid)fullInfo.OrganizationId))
                //{
                //    return new ChangePosterStatusResponse()
                //    {
                //        IsSuccess = false,
                //        StatusCode = HttpStatusCode.NotFound,
                //        ErrorMessage = "Organization is empty or not found"
                //    };
                //}

                if (poster.CategoryId == Guid.Empty || fullInfo.City == null || fullInfo.City == Guid.Empty || string.IsNullOrEmpty(fullInfo.AgeRestriction) || string.IsNullOrEmpty(fullInfo.TimeStart))
                {
                    return new ChangePosterStatusResponse()
                    {
                        IsSuccess = false,
                        StatusCode = HttpStatusCode.NotFound,
                        ErrorMessage = "Required fields is empty"
                    };
                }

                if (poster.ReleaseDate < DateTime.UtcNow.Date)
                {
                    return new ChangePosterStatusResponse()
                    {
                        IsSuccess = false,
                        StatusCode = HttpStatusCode.NotFound,
                        ErrorMessage = "Release date is incorrect"
                    };
                }

                PosterMultilangModel ml = null;
                foreach(var lang in Constants.SystemLangs)
                {
                    ml = await _repository.GetMultilangPoster(request.Id, lang);
                    if (ml == null)
                    {
                        return new ChangePosterStatusResponse()
                        {
                            IsSuccess = false,
                            StatusCode = HttpStatusCode.NotFound,
                            ErrorMessage = "Poster on lang '" + lang + "' not found"
                        };
                    }

                    if (string.IsNullOrEmpty(ml.Place) || string.IsNullOrEmpty(ml.Description))
                    {
                        return new ChangePosterStatusResponse()
                        {
                            IsSuccess = false,
                            StatusCode = HttpStatusCode.NotFound,
                            ErrorMessage = "Required fields is empty on lang '" + lang + "'"
                        };
                    }
                }
            }
            
            poster.Status = (int)request.Status;
            await _repository.UpdatePoster(poster, _user);

            return new ChangePosterStatusResponse();
        }
    }
}
