using InfoPoster_backend.Handlers.Administration.Organization;
using InfoPoster_backend.Handlers.Administration.Poster;
using InfoPoster_backend.Models.Administration;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Tools;
using MediatR;

namespace InfoPoster_backend.Handlers.Administration.Offer
{
    public class AddPopularityOfferRequest : IRequest<AddPopularityOfferResponse>
    {
        public POPULARITY_PLACE Place { get; set; }
        public Guid CityId { get; set; }
        public List<PopularityRequestModel> Popularity { get; set; }
    }

    public class AddPopularityOfferResponse
    {

    }

    public class AddPopularityOfferHandler : IRequestHandler<AddPopularityOfferRequest, AddPopularityOfferResponse>
    {
        private readonly OfferRepository _repository;
        
        public AddPopularityOfferHandler(OfferRepository repository)
        {
            _repository = repository;
        }

        public async Task<AddPopularityOfferResponse> Handle(AddPopularityOfferRequest request, CancellationToken cancellation = default)
        {

            var popularity = await _repository.GetPopularityList(request.Place, request.CityId);
            var addList = new List<PopularityModel>();

            foreach (var item in request.Popularity)
            {
                addList.Add(new PopularityModel()
                {
                    Id = Guid.NewGuid(),
                    ApplicationId = item.Id,
                    Place = request.Place,
                    Popularity = item.Popularity,
                    Type = POPULARITY_TYPE.OFFER,
                    CityId = request.CityId
                });
            }

            await _repository.RemovePopularity(popularity);
            await _repository.AddPopularity(addList);
            return new AddPopularityOfferResponse();
        }
    }
}
