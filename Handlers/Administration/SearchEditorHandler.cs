using InfoPoster_backend.Repos;
using MediatR;

namespace InfoPoster_backend.Handlers.Administration
{
    public class SearchEditorRequest : IRequest<List<SearchEditorResponse>>
    {
        public string SearchText { get; set; }
    }

    public class SearchEditorResponse
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
    }

    public class SearchEditorHandler : IRequestHandler<SearchEditorRequest, List<SearchEditorResponse>>
    {
        private readonly AccountRepository _account;
        public SearchEditorHandler(AccountRepository account)
        {
            _account = account;
        }

        public async Task<List<SearchEditorResponse>> Handle(SearchEditorRequest request, CancellationToken cancellationToken = default)
        {
            var users = await _account.SearchUsers(request.SearchText);
            var result = users.Select(u => new SearchEditorResponse()
            {
                Id = u.Id,
                Username = u.FirstName + " " + u.LastName,
            }).ToList();

            return result;
        }
    }
}
