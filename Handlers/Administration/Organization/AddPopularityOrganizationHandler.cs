using InfoPoster_backend.Models.Administration;
using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Administration.Organization
{
    public class AddPopularityOrganizationRequest : IRequest<AddPopularityOrganizationResponse>
    {
        public POPULARITY_PLACE Place { get; set; }
        public List<PopularityRequestModel> Popularity { get; set; }
    }

    public class PopularityRequestModel
    {
        public Guid Id { get; set; }
        public int Popularity { get; set; }
    }

    public class AddPopularityOrganizationResponse
    {
        
    }

    public class AddPopularityOrganizationHandler : IRequestHandler<AddPopularityOrganizationRequest, AddPopularityOrganizationResponse>
    {
        private readonly OrganizationRepository _repository;

        public AddPopularityOrganizationHandler(OrganizationRepository repository)
        {
            _repository = repository;
        }

        public async Task<AddPopularityOrganizationResponse> Handle(AddPopularityOrganizationRequest request, CancellationToken cancellation = default)
        {
            var anyIdentical = request.Popularity.GroupBy(p => p.Id).Select(p => new { Id = p.Key, Count = p.Count() }).Any(p => p.Count > 1);
            if (anyIdentical)
            {
                return null;
            }

            var popularity = await _repository.GetPopularityList(request.Place);
            var removeList = new List<PopularityModel>();
            var addList = new List<PopularityModel>();

            foreach (var item in request.Popularity)
            {
                if (popularity.Any(p => p.ApplicationId == item.Id))
                {
                    var remove = popularity.FirstOrDefault(p => p.ApplicationId == item.Id);
                    removeList.Add(remove);
                }

                addList.Add(new PopularityModel()
                {
                    Id = Guid.NewGuid(),
                    ApplicationId = item.Id,
                    Place = request.Place,
                    Popularity = item.Popularity
                });
            }

            await _repository.RemovePopularity(removeList);
            await _repository.AddPopularity(addList);
            return new AddPopularityOrganizationResponse();
        }
    }
}
