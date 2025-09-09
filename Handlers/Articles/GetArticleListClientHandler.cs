using InfoPoster_backend.Models;
using InfoPoster_backend.Models.Selectel;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Login;
using InfoPoster_backend.Services.Selectel_API;
using InfoPoster_backend.Tools;
using MediatR;

namespace InfoPoster_backend.Handlers.Articles
{
    public class GetArticleListClientRequest : IRequest<GetArticleListClientResponse>
    {
        public int Limit { get; set; }
        public int Offset { get; set; }
    }

    public class GetArticleListClientResponse
    {
        public List<ArticleResponse> Data { get; set; }
        public int Limit { get; set; }
        public int Offset { get; set; }
        public int Total { get; set; }
    }

    public class GetArticleListClientHandler : IRequestHandler<GetArticleListClientRequest, GetArticleListClientResponse>
    {
        private readonly ArticleRepository _repository;
        private readonly SelectelAuthService _selectelAuth;
        private readonly FileRepository _file;

        public GetArticleListClientHandler(ArticleRepository repository, SelectelAuthService selectelAuth, FileRepository file)
        {
            _repository = repository;
            _selectelAuth = selectelAuth;
            _file = file;
        }

        public async Task<GetArticleListClientResponse> Handle(GetArticleListClientRequest request, CancellationToken cancellationToken = default)
        {
            var list = await _repository.GetArticleList(Models.Posters.POSTER_STATUS.PUBLISHED);

            var total = list.Count;
            list = list.Skip(request.Offset).Take(request.Limit).Select(a => new ArticleResponse()
            {
                Body = a.Body,
                ShortDescription = a.ShortDescription,
                Id = a.Id,
                Lang = a.Lang,
                Title = a.Title,
                UserId = a.UserId,
                UserName = a.UserName,
                Status = a.Status,
                CreatedAt = a.CreatedAt
            }).OrderByDescending(a => a.CreatedAt).ToList();

            var loggedIn = await _selectelAuth.Login();
            var selectelUUID = string.Empty;
            if (loggedIn)
            {
                selectelUUID = await _selectelAuth.GetContainerUUID("dosdoc");
                foreach (var article in list)
                {
                    var files = await _file.GetSelectelFiles(article.Id, (int)FILE_PLACES.GALLERY);
                    var primaryFile = await _file.GetPrimaryFile(article.Id, (int)FILE_PLACES.GALLERY);
                    var file = primaryFile != null ? primaryFile.FileId : files.Select(f => f.Id).FirstOrDefault();

                    article.FileURL = string.Concat("https://", selectelUUID, ".selstorage.ru/", file);
                }
            }

            var result = new GetArticleListClientResponse()
            {
                Data = list,
                Total = total,
                Limit = request.Limit,
                Offset = request.Offset,
            };
            return result;
        }
    }
}
