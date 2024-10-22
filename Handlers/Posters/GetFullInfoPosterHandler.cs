using InfoPoster_backend.Handlers.Administration;
using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Selectel_API;
using MediatR;

namespace InfoPoster_backend.Handlers.Posters
{
    public class GetFullInfoPosterRequest : IRequest<PosterFullInfoResponseModel>
    {
        public Guid Id { get; set; }
    }

    public class GetFullInfoPosterHandler : IRequestHandler<GetFullInfoPosterRequest, PosterFullInfoResponseModel>
    {
        private readonly PosterRepository _repository;
        private readonly FileRepository _fileRepository;
        private readonly SelectelAuthService _selectelAuthService;
        private readonly string _lang;

        public GetFullInfoPosterHandler(PosterRepository repository, FileRepository fileRepository, SelectelAuthService selectel, IHttpContextAccessor accessor)
        {
            _repository = repository;
            _fileRepository = fileRepository;
            _selectelAuthService = selectel;
            _lang = accessor.HttpContext.Items["ClientLang"].ToString().ToLower();
        }

        public async Task<PosterFullInfoResponseModel> Handle(GetFullInfoPosterRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _repository.GetFullInfo(request.Id, _lang);
            var files = await _fileRepository.GetSelectelFiles(request.Id, (int)FILE_PLACES.GALLERY);

            var loggedIn = await _selectelAuthService.Login();

            result.GaleryUrls = new List<GetFileResponse>();
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
                        URL = imageSrc
                    };
                    result.GaleryUrls.Add(response);
                }
            }

            var viewLog = new PosterViewLogModel()
            {
                Id = Guid.NewGuid(),
                PosterId = request.Id,
                Date = DateTime.UtcNow
            };

            await _repository.AddViewLog(viewLog);
            return result;
        }
    }
}
