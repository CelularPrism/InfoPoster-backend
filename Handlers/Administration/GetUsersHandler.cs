using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Login;
using MediatR;

namespace InfoPoster_backend.Handlers.Administration
{
    public class GetUsersRequest : IRequest<List<GetUsersResponse>> { }

    public class GetUsersResponse
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public bool IsBlocked { get; set; }
    }

    public class GetUsersHandler : IRequestHandler<GetUsersRequest, List<GetUsersResponse>>
    {
        private readonly AccountRepository _repository;
        private readonly Guid _userId;

        public GetUsersHandler(AccountRepository repository, LoginService loginService)
        {
            _repository = repository;
            _userId = loginService.GetUserId();
        }

        public async Task<List<GetUsersResponse>> Handle(GetUsersRequest request, CancellationToken cancellationToken = default)
        {
            var users = await _repository.GetUsersForAdmin();
            var result = users.Select(u => new GetUsersResponse()
                              {
                                  Id = u.Id,
                                  CreatedAt = u.CreatedAt,
                                  Email = u.Email,
                                  IsBlocked = u.IsBlocked,
                                  UserName = u.FirstName + " " + u.LastName
                              }).ToList();
            return result;
        }
    }
}
