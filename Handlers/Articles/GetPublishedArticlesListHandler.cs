using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Selectel;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Selectel_API;
using InfoPoster_backend.Tools;
using MediatR;

namespace InfoPoster_backend.Handlers.Articles
{
    public class GetPublishedArticlesListRequest : IRequest<List<ArticleResponse>>
    {
        public Models.Administration.POPULARITY_PLACE Place { get; set; }
    }

    public class GetPublishedArticlesListHandler : IRequestHandler<GetPublishedArticlesListRequest, List<ArticleResponse>>
    {
        private readonly ArticleRepository _repository;
        private readonly FileRepository _file;
        private readonly SelectelAuthService _selectelAuth;
        private readonly string _lang;

        public GetPublishedArticlesListHandler(ArticleRepository repository, FileRepository file, SelectelAuthService selectelAuth, IHttpContextAccessor accessor)
        {
            _repository = repository;
            _file = file;
            _selectelAuth = selectelAuth;
            _lang = accessor.HttpContext.Items[Constants.HTTP_ITEM_ClientLang].ToString();
        }

        public async Task<List<ArticleResponse>> Handle(GetPublishedArticlesListRequest request, CancellationToken cancellation = default)
        {
            var popular = await _repository.GetPopularArticleList(request.Place);
            var published = await _repository.GetArticleListByStatus(Models.Posters.POSTER_STATUS.PUBLISHED);

            var nonPopular = published.Where(p => !popular.Select(pop => pop.Id).Contains(p.Id) && p.Lang == _lang)
                                      .Select(p => new ArticleResponse()
                                      {
                                          Id = p.Id,
                                          CreatedAt = p.CreatedAt,
                                          Body = p.Body,
                                          Lang = p.Lang,
                                          ShortDescription = p.ShortDescription,
                                          Status = (int)p.Status,
                                          Title = p.Title
                                      }).OrderByDescending(p => p.CreatedAt).ToList();

            var result = popular.Select(p => new ArticleResponse()
            {
                Id = p.Id,
                CreatedAt = p.CreatedAt,
                Body = p.Body,
                Lang = p.Lang,
                ShortDescription = p.ShortDescription,
                Status = (int)p.Status,
                Title = p.Title
            }).ToList();
            result.AddRange(nonPopular);

            result = result.Take(3).ToList();

            var loggedIn = await _selectelAuth.Login();
            if (loggedIn)
            {
                var selectelUUID = await _selectelAuth.GetContainerUUID("dosdoc");
                foreach (var item in result)
                {
                    var file = await _file.GetPrimaryFile(item.Id, (int)FILE_PLACES.GALLERY);
                    if (file == null)
                        file = await _file.GetApplicationFileByApplication(item.Id);

                    if (file != null)
                    {
                        item.FileURL = string.Concat("https://", selectelUUID, ".selstorage.ru/", file.FileId);
                    }
                }
            }

            return result;
        }
    }
}
