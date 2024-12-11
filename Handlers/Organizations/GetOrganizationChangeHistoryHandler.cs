using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Organizations
{
    public class GetOrganizationChangeHistoryRequest : IRequest<List<GetOrganizationChangeHistoryResponse>>
    {
        public Guid Id { get; set; }
    }

    public class GetOrganizationChangeHistoryResponse
    {
        public Guid Id { get; set; }
        public string FieldName { get; set; }
        public string ChangeInfo { get; set; }
    }

    public class GetOrganizationChangeHistoryHandler : IRequestHandler<GetOrganizationChangeHistoryRequest, List<GetOrganizationChangeHistoryResponse>> 
    {
        private readonly OrganizationRepository _repository;

        public GetOrganizationChangeHistoryHandler(OrganizationRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<GetOrganizationChangeHistoryResponse>> Handle(GetOrganizationChangeHistoryRequest request, CancellationToken cancellationToken = default)
        {
            var history = await _repository.GetChangeHistory(request.Id);
            if (history.Count == 0)
                return null;

            var response = history.Select(h => new GetOrganizationChangeHistoryResponse()
            {
                Id = h.Id,
                FieldName = h.FieldName,
                ChangeInfo = !string.IsNullOrEmpty(h.OldValue) || !string.IsNullOrEmpty(h.NewValue) ? h.OldValue + " to " + h.NewValue : null,
            }).ToList();

            return response;
        }
    }
}
