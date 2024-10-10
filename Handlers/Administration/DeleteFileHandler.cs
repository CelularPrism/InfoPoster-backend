using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Administration
{
    public class DeleteFileRequest : IRequest<DeleteFileResponse>
    {
        public Guid FileId { get; set; }
        public Guid ApplicationId { get; set; }
    }

    public class DeleteFileResponse { }

    public class DeleteFileHandler : IRequestHandler<DeleteFileRequest, DeleteFileResponse>
    {
        private readonly FileRepository _repository;

        public DeleteFileHandler(FileRepository repository)
        {
            _repository = repository;
        }

        public async Task<DeleteFileResponse> Handle(DeleteFileRequest request, CancellationToken cancellationToken = default)
        {
            await _repository.RemoveFile(request.FileId, request.ApplicationId);
            return new DeleteFileResponse();
        }
    }
}
