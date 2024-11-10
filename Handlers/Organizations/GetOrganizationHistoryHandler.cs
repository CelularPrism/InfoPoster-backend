using InfoPoster_backend.Models;
using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Organizations
{
    public class GetOrganizationHistoryRequest : IRequest<List<ApplicationHistoryResponse>>
    {
        public Guid Id { get; set; }
    }

    public class GetOrganizationHistoryHandler : IRequestHandler<GetOrganizationHistoryRequest, List<ApplicationHistoryResponse>> 
    {
        private readonly OrganizationRepository _repository;

        public GetOrganizationHistoryHandler(OrganizationRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<ApplicationHistoryResponse>> Handle(GetOrganizationHistoryRequest request, CancellationToken cancellationToken = default)
        {
            var organizatioHistory = await _repository.GetHistoryList(request.Id);
            if (organizatioHistory == null)
            {
                return null;
            }

            return organizatioHistory;
        }
    }
}
