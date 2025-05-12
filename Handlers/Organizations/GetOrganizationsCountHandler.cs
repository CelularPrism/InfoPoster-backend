using InfoPoster_backend.Repos;
using InfoPoster_backend.Tools;
using MediatR;

namespace InfoPoster_backend.Handlers.Organizations
{
    public class GetOrganizationsCountRequest : IRequest<GetOrganizationsCountResponse> { }

    public class GetOrganizationsCountResponse
    {
        public string City { get; set; }
        public int Count { get; set; }
    }

    public class GetOrganizationsCountHandler : IRequestHandler<GetOrganizationsCountRequest, GetOrganizationsCountResponse> 
    {
        private readonly OrganizationRepository _repository;
        private readonly Guid _city;

        public GetOrganizationsCountHandler(OrganizationRepository repository, IHttpContextAccessor accessor)
        {
            _repository = repository;
            _city = Guid.TryParse(accessor.HttpContext.Request.Headers["X-Testing"].ToString(), out _city) ? Guid.Parse(accessor.HttpContext.Request.Headers["X-Testing"].ToString()) : Constants.DefaultCity;
        }

        public async Task<GetOrganizationsCountResponse> Handle(GetOrganizationsCountRequest request, CancellationToken cancellationToken = default)
        {
            var city = await _repository.GetCityName(_city);
            var count = await _repository.GetCountByCity(_city);
            var round = Math.Round(count / 10.0, 0, MidpointRounding.ToNegativeInfinity);
            var result = new GetOrganizationsCountResponse()
            {
                City = city,
                Count = (int)round * 10
            };

            return result;
        }
    }
}
