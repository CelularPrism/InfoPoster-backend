using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Posters
{
    public class GetPosterChangeHistoryRequest : IRequest<List<GetPosterChangeHistoryResponse>>
    {
        public Guid Id { get; set; }
    }

    public class GetPosterChangeHistoryResponse
    {
        public Guid Id { get; set; }
        public string FieldName { get; set; }
        public string ChangeInfo { get; set; }
    }

    public class GetPosterChangeHistoryHandler : IRequestHandler<GetPosterChangeHistoryRequest, List<GetPosterChangeHistoryResponse>>
    {
        private readonly PosterRepository _repository;

        public GetPosterChangeHistoryHandler(PosterRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<GetPosterChangeHistoryResponse>> Handle(GetPosterChangeHistoryRequest request, CancellationToken cancellationToken = default)
        {
            var history = await _repository.GetChangeHistory(request.Id);
            if (history.Count == 0)
                return null;

            var response = history.Select(h => new GetPosterChangeHistoryResponse()
            {
                Id = h.Id,
                FieldName = h.FieldName,
                ChangeInfo = !string.IsNullOrEmpty(h.OldValue) || !string.IsNullOrEmpty(h.NewValue) ? h.OldValue + " to " + h.NewValue : null,
            }).ToList();

            return response;
        }
    }
}
