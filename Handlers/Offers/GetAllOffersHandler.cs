using InfoPoster_backend.Models.Offers;
using InfoPoster_backend.Models.Organizations;
using InfoPoster_backend.Models.Posters;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Login;
using InfoPoster_backend.Tools;
using MediatR;

namespace InfoPoster_backend.Handlers.Offers
{
    public class GetAllOffersRequest : IRequest<GetAllOffersResponse>
    {
        public int Sort { get; set; }
        public OFFER_TYPES? Type { get; set; }
        public Guid? CityId { get; set; }
        public POSTER_STATUS? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid? UserId { get; set; }
        public int Page { get; set; }
        public int CountPerPage { get; set; }
    }

    public class GetAllOffersResponse
    {
        public List<AllOfferModel> Offers { get; set; }
        public int Count { get; set; }
        public int Page { get; set; }
        public int CountPerPage { get; set; }
    }

    public class AllOfferModel
    {
        public AllOfferModel() { }

        public AllOfferModel(OffersModel offer, OffersMultilangModel multilang, string userName)
        {
            Id = offer.Id;
            Name = multilang.Name;
            CreatedAt = offer.CreatedAt;
            Type = offer.Type;
            UserId = offer.UserId;
            CreatedBy = userName;
            Status = offer.Status;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public OFFER_TYPES Type { get; set; }
        public Guid UserId { get; set; }
        public string CreatedBy { get; set; }
        public POSTER_STATUS Status { get; set; }
        public Guid? CityId { get; set; }
        public string CityName { get; set; }
    }

    public class GetAllOffersHandler : IRequestHandler<GetAllOffersRequest, GetAllOffersResponse> 
    {
        private readonly OfferRepository _repository;
        private readonly Guid _user;
        private readonly string _lang;

        public GetAllOffersHandler(OfferRepository repository, LoginService loginService, IHttpContextAccessor accessor)
        {
            _repository = repository;
            _user = loginService.GetUserId();
            _lang = accessor.HttpContext.Items[Constants.HTTP_ITEM_ClientLang].ToString();
        }

        public async Task<GetAllOffersResponse> Handle(GetAllOffersRequest request, CancellationToken cancellationToken = default)
        {
            var availableStatuses = new List<POSTER_STATUS>()
            {
                POSTER_STATUS.PENDING,
                POSTER_STATUS.PUBLISHED,
                POSTER_STATUS.DRAFT,
                POSTER_STATUS.REJECTED,
                POSTER_STATUS.REVIEWING
            };

            if (request.Status != null)
            {
                availableStatuses = new List<POSTER_STATUS>()
                {
                    request.Status.Value
                };
            }

            var offers = await _repository.GetOfferList(availableStatuses, request.Type, request.StartDate, request.EndDate, request.UserId, request.CityId);
            var result = new GetAllOffersResponse()
            {
                Count = offers.Count,
                CountPerPage = request.CountPerPage,
                Page = request.Page + 1,
            };

            if (request.Sort == 0)
            {
                offers = offers.OrderByDescending(x => x.CreatedAt).ToList();
            }
            else
            {
                offers = offers.OrderBy(x => x.Status).ToList();
            }

            offers = offers.Skip(request.Page * request.CountPerPage).Take(request.CountPerPage).ToList();
            var cities = await _repository.GetCities(_lang);

            var offerIds = offers.Select(o => o.Id).AsEnumerable();
            var userIds = offers.Select(o => o.UserId).AsEnumerable();
            var multilang = await _repository.GetMultilangList(offerIds, _lang);
            var users = await _repository.GetUserList(userIds);

            var offersList = offers.Select(o => new AllOfferModel()
            {
                CityId = o.CityId,
                CityName = cities.Where(x => x.Id == o.CityId).Select(x => x.Name).FirstOrDefault(),
                Id = o.Id,
                CreatedAt = o.CreatedAt,
                CreatedBy = users.Where(u => u.Id == o.UserId).Select(u => u.FirstName + " " + u.LastName).FirstOrDefault(),
                UserId = o.UserId,
                Name = multilang.Where(m => m.OfferId == o.Id).Select(m => m.Name).FirstOrDefault(),
                Status = o.Status,
                Type = o.Type
            }).ToList();

            result.Offers = offersList.OrderByDescending(o => o.CreatedAt).ToList();

            return result;
        }
    }
}
