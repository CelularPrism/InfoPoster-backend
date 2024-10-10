using InfoPoster_backend.Models.Selectel;
using InfoPoster_backend.Models;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Selectel_API;
using MediatR;
using System.Buffers.Text;

namespace InfoPoster_backend.Handlers.Administration
{
    public class UploadFileRequest : IRequest<UploadFileResponse>
    {
        public Guid ApplicationId { get; set; }
        public string Base64 { get; set; }
        public string Type { get; set; }
    }

    public class UploadFileResponse { }

    public class UploadFileHandler : IRequestHandler<UploadFileRequest, UploadFileResponse> 
    {
        private readonly SelectelAuthService _selectelAuthService;
        private readonly FileRepository _repository;

        public UploadFileHandler(SelectelAuthService selectelAuthService, FileRepository repository)
        {
            _selectelAuthService = selectelAuthService;
            _repository = repository;
        }

        public async Task<UploadFileResponse> Handle(UploadFileRequest request, CancellationToken cancellationToken = default)
        {
            var loggedIn = await _selectelAuthService.Login();
            if (loggedIn)
            {

                var file = new SelectelFileURLModel()
                {
                    Type = request.Type
                };

                var app = new FileToApplication()
                {
                    ApplicationId = request.ApplicationId,
                    FileId = file.Id
                };

                await _repository.AddSelectelFile(file);
                await _repository.AddFileToApplication(app);
                await _selectelAuthService.UploadObject(Convert.FromBase64String(request.Base64), file.Id);
            }

            return new UploadFileResponse();
        }
    }
}
