using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Login;
using MediatR;

namespace InfoPoster_backend.Handlers.Account
{
    public class UpdateUserRequest : IRequest<UpdateUserResponse>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string ImageSrc { get; set; }
    }

    public class UpdateUserResponse { }

    public class UpdateUserHandler : IRequestHandler<UpdateUserRequest, UpdateUserResponse>
    {
        private readonly AccountRepository _repository;
        private readonly LoginService _login;

        public UpdateUserHandler(AccountRepository repository, LoginService login)
        {
            _repository = repository;
            _login = login;
        }

        public async Task<UpdateUserResponse> Handle(UpdateUserRequest request, CancellationToken cancellationToken = default)
        {
            var userId = _login.GetUserId();
            var user = await _repository.GetUser(userId);

            if (!string.IsNullOrEmpty(request.FirstName) && user.FirstName != request.FirstName)
                user.FirstName = request.FirstName;

            if (!string.IsNullOrEmpty(request.LastName) && user.LastName != request.LastName)
                user.LastName = request.LastName;

            if (!string.IsNullOrEmpty(request.Phone) && user.Phone != request.Phone)
                user.Phone = request.Phone;

            if (!string.IsNullOrEmpty(request.ImageSrc) && user.ImageSrc != request.ImageSrc)
                user.ImageSrc = request.ImageSrc;

            await _repository.UpdateUser(user);
            return new UpdateUserResponse();
        }
    }
}
