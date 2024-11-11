using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Administration
{
    public class FileSetPrimaryRequest : IRequest<FileSetPrimaryResponse>
    {
        public Guid Id { get; set; }
    }

    public class FileSetPrimaryResponse { }

    public class FileSetPrimaryHandler : IRequestHandler<FileSetPrimaryRequest, FileSetPrimaryResponse>
    {
        private readonly FileRepository _repository;
        public FileSetPrimaryHandler(FileRepository repository)
        {
            _repository = repository;
        }

        public async Task<FileSetPrimaryResponse> Handle(FileSetPrimaryRequest request, CancellationToken cancellationToken = default)
        {
            var file = await _repository.GetApplicationFile(request.Id);
            if (file == null)
            {
                return null;
            }

            file.IsPrimary = true;
            await _repository.UpdateFileToApplication(file);
            return new FileSetPrimaryResponse();
        }
    }
}
