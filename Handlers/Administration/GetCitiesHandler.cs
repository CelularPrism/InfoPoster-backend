using InfoPoster_backend.Models.Cities;
using InfoPoster_backend.Models.Contexts;
using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Administration
{
    public class GetCitiesRequest : IRequest<List<CityModel>> { }

    public class GetCitiesHandler : IRequestHandler<GetCitiesRequest, List<CityModel>> 
    {
        private readonly OrganizationRepository _repository;
        private readonly string _lang;

        public GetCitiesHandler(OrganizationRepository repository, IHttpContextAccessor accessor)
        {
            _repository = repository;
            _lang = accessor.HttpContext.Items["ClientLang"].ToString();
        }

        public async Task<List<CityModel>> Handle(GetCitiesRequest request, CancellationToken cancellationToken = default) =>
            await _repository.GetCities(_lang);
    }
}
