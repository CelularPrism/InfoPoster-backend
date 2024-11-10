using InfoPoster_backend.Models;
using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Administration
{
    public class GetPosterHistoryRequest : IRequest<List<ApplicationHistoryResponse>>
    {
        public Guid Id { get; set; }
    }

    public class GetPosterHistoryHandler : IRequestHandler<GetPosterHistoryRequest, List<ApplicationHistoryResponse>>
    {
        private readonly PosterRepository _repository;

        public GetPosterHistoryHandler(PosterRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ApplicationHistoryResponse>> Handle(GetPosterHistoryRequest request, CancellationToken cancellationToken = default)
        {
            var response = await _repository.GetHistoryList(request.Id);
            if (response == null)
            {
                return null;
            }

            return response;
        }
    }
}
