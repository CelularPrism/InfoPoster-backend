using InfoPoster_backend.Handlers.Administration.Organization;
using InfoPoster_backend.Models.Administration;
using InfoPoster_backend.Repos;
using MediatR;
using Org.BouncyCastle.Asn1.Ocsp;

namespace InfoPoster_backend.Handlers.Administration.Poster
{
    public class AddPopularityPosterRequest : IRequest<AddPopularityPosterResponse>
    {
        public POPULARITY_PLACE Place { get; set; }
        public List<PopularityRequestModel> Popularity { get; set; }
    }

    public class AddPopularityPosterResponse
    {

    }

    public class AddPopularityPosterHandler : IRequestHandler<AddPopularityPosterRequest, AddPopularityPosterResponse>
    {
        private readonly PosterRepository _repository;

        public AddPopularityPosterHandler(PosterRepository repository)
        {
            _repository = repository;
        }

        public async Task<AddPopularityPosterResponse> Handle(AddPopularityPosterRequest request, CancellationToken cancellation = default)
        {

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
            return new AddPopularityPosterResponse();
        }
    }
}
