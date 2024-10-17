using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Selectel_API;
using MediatR;

namespace InfoPoster_backend.Handlers.Administration
{
    public class GetFileRequest : IRequest<List<GetFileResponse>>
    {
        public Guid ApplicationId { get; set; }
        public int Place { get; set; }
    }

    public class GetFileResponse
    {
        public Guid Id { get; set; }
        public string URL { get; set; }
        public string Type { get; set; }
    }

    public class GetFileHandler : IRequestHandler<GetFileRequest, List<GetFileResponse>>
    {
        private readonly FileRepository _repository;
        private readonly SelectelAuthService _selectelAuthService;

        public GetFileHandler(FileRepository repository, SelectelAuthService selectelAuthService)
        {
            _repository = repository;
            _selectelAuthService = selectelAuthService;
        }

        public async Task<List<GetFileResponse>> Handle(GetFileRequest request, CancellationToken cancellationToken = default)
        {
            var files = await _repository.GetSelectelFiles(request.ApplicationId, request.Place);
            var loggedIn = await _selectelAuthService.Login();

            var result = new List<GetFileResponse>();
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
                    result.Add(response);
                }
            }
            return result;
        }
    }
}
