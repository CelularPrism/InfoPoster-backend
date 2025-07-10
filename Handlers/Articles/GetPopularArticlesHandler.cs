using InfoPoster_backend.Models;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Selectel_API;
using MediatR;

namespace InfoPoster_backend.Handlers.Articles
{
    public class GetPopularArticlesRequest : IRequest<List<ArticleResponse>>
    {
        public int Page { get; set; }
        public int CountPerPage { get; set; }
    }

    public class GetPopularArticlesHandler : IRequestHandler<GetPopularArticlesRequest, List<ArticleResponse>>
    {
        private readonly ArticleRepository _repository;
        private readonly FileRepository _file;
        private readonly SelectelAuthService _selectel;

        public GetPopularArticlesHandler(ArticleRepository repository, FileRepository file, SelectelAuthService selectel)
        {
            _repository = repository;
            _file = file;
            _selectel = selectel;
        }

        public async Task<List<ArticleResponse>> Handle(GetPopularArticlesRequest request, CancellationToken cancellation = default)
        {
            var articles = await _repository.GetPopularArticleList(Models.Administration.POPULARITY_PLACE.MAIN);
            var result = new List<ArticleResponse>();
            var isLoggedIn = await _selectel.Login();

            if (isLoggedIn)
            {
                var selectelUUID = await _selectel.GetContainerUUID("dosdoc");
                foreach (var item in result)
                {
                    var file = await _file.GetPrimaryFile(item.Id, 0);
                    if (file == null)
                        file = await _file.GetApplicationFileByApplication(item.Id);

                    item.FileURL = string.Concat("https://", selectelUUID, ".selstorage.ru/", file.FileId);
                }
            }
            return result;


        }
    }
}
