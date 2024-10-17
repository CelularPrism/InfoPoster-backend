using InfoPoster_backend.Models.Organizations.Menu;
using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Organizations
{
    public class GetMenusRequest : IRequest<List<MenuModel>> { }

    public class GetMenusHandler : IRequestHandler<GetMenusRequest, List<MenuModel>>
    {
        private readonly OrganizationRepository _repository;
        private string _lang;

        public GetMenusHandler(OrganizationRepository repository, IHttpContextAccessor accessor)
        {
            _repository = repository;
            _lang = accessor.HttpContext.Items["ClientLang"].ToString();
        }

        public async Task<List<MenuModel>> Handle(GetMenusRequest request, CancellationToken cancellationToken = default)
        {
            var result = await _repository.GetMenuList(_lang);
            return result;
        }
    }
}
