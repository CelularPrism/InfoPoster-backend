using InfoPoster_backend.Handlers.Administration;
using InfoPoster_backend.Models;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Selectel_API;
using MediatR;

namespace InfoPoster_backend.Handlers.Articles
{
    public class GetArticleRequest : IRequest<ArticleResponse>
    {
        public Guid Id { get; set; }
    }

    public class GetArticleHandler : IRequestHandler<GetArticleRequest, ArticleResponse>
    {
        private readonly ArticleRepository _article;
        private readonly AccountRepository _account;
        private readonly FileRepository _file;
        private readonly SelectelAuthService _selectelAuthService;

        public GetArticleHandler(ArticleRepository article, AccountRepository account, FileRepository file, SelectelAuthService selectelAuthService)
        {
            _article = article;
            _account = account;
            _file = file;
            _selectelAuthService = selectelAuthService;
        }

        public async Task<ArticleResponse> Handle(GetArticleRequest request, CancellationToken cancellationToken)
        {
            var article = await _article.GetArticle(request.Id);
            if (article == null) 
                return null;

            var user = await _account.GetUser(article.UserId);
            var result = new ArticleResponse()
            {
                UserId = user.Id,
                Title = article.Title,
                Body = article.Body,
                Id = article.Id,
                Lang = article.Lang,
                UserName = user.FirstName + " " + user.LastName,
                Status = (int)article.Status,
                CreatedAt = article.CreatedAt
            };

            var loggedIn = await _selectelAuthService.Login();
            var files = await _file.GetSelectelFiles(request.Id, (int)FILE_PLACES.GALLERY);
            var primaryFile = await _file.GetPrimaryFile(request.Id, (int)FILE_PLACES.GALLERY);
            if (loggedIn)
            {
                var selectelUUID = await _selectelAuthService.GetContainerUUID("dosdoc");
                var imageSrc = string.Empty;
                GetFileResponse response = null;
                foreach (var file in files)
                {
                    imageSrc = string.Concat("https://", selectelUUID, ".selstorage.ru/", file.Id);
                    response = new GetFileResponse()
                    {
                        Id = file.Id,
                        Type = file.Type,
                        URL = imageSrc,
                        IsPrimary = primaryFile != null && primaryFile.FileId == file.Id ? true : false,
                    };
                    result.GaleryUrls.Add(response);
                }
                result.GaleryUrls = result.GaleryUrls.OrderByDescending(f => f.IsPrimary).ToList();
            }

            return result;
        }
    }
}
