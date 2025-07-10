using InfoPoster_backend.Models;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Login;
using InfoPoster_backend.Tools;
using MediatR;

namespace InfoPoster_backend.Handlers.Articles
{
    public class GetArticleListRequest : IRequest<GetArticleListResponse> 
    { 
        public int Page { get; set; }
        public int CountPerPage { get; set; }
    }

    public class GetArticleListResponse
    {
        public List<ArticleResponse> Data { get; set; }
        public int Page { get; set; }
        public int CountPerPage { get; set; }
        public int Total { get; set; }
    }

    public class GetArticleListHandler : IRequestHandler<GetArticleListRequest, GetArticleListResponse>
    {
        private readonly ArticleRepository _repository;
        private readonly Guid _user;

        public GetArticleListHandler(ArticleRepository repository, LoginService loginService)
        {
            _repository = repository;
            _user = loginService.GetUserId();
        }

        public async Task<GetArticleListResponse> Handle(GetArticleListRequest request, CancellationToken cancellationToken = default)
        {
            var list = new List<ArticleResponse>();

            var roles = await _repository.GetRoles(_user);
            var isNotModerator = roles.Any(r => r == Constants.ROLE_ADMIN || r == Constants.ROLE_EDITOR);

            if (isNotModerator)
            {
                var isAdmin = roles.Any(r => r == Constants.ROLE_ADMIN);
                if (isAdmin)
                    list = await _repository.GetArticleList(_user);
                else
                    list = await _repository.GetArticleList();
            } else
            {
                list = await _repository.GetArticleList(Models.Posters.POSTER_STATUS.DRAFT);
            }

            var total = list.Count;
            list = list.Skip(request.Page * request.CountPerPage).Take(request.CountPerPage).Select(a => new ArticleResponse()
            {
                Body = !string.IsNullOrEmpty(a.Body) ? a.Body.Substring(0, 30) : string.Empty,
                Id = a.Id,
                Lang = a.Lang,
                Title = a.Title,
                UserId = a.UserId,
                UserName = a.UserName,
                Status = a.Status,
                CreatedAt = a.CreatedAt
            }).ToList();

            var result = new GetArticleListResponse()
            {
                Data = list,
                Total = total,
                Page = request.Page + 1,
                CountPerPage = request.CountPerPage,
            };
            return result;
        }
    }
}
