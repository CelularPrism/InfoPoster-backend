using InfoPoster_backend.Models.Account;
using InfoPoster_backend.Repos;
using InfoPoster_backend.Services.Login;
using MediatR;

namespace InfoPoster_backend.Handlers.Account
{
    public class LoginRequestModel : IRequest<LoginResponseModel>
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponseModel
    {
        public LoginResponseModel(UserModel user, string token)
        {
            Id = user.Id;
            Email = user.Email;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Phone = user.Phone;
            ImageSrc = user.ImageSrc;
            Token = token;
        }

        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string ImageSrc { get; set; }
        public string Token { get; set; }
        public Guid RoleId { get; set; }
    }

    public class LoginRequestHandler : IRequestHandler<LoginRequestModel, LoginResponseModel>
    {
        private readonly LoginService _login;
        private readonly AccountRepository _repository;

        public LoginRequestHandler(LoginService login, AccountRepository repository)
        {
            _login = login;
            _repository = repository;
        }

        public async Task<LoginResponseModel> Handle(LoginRequestModel request, CancellationToken cancellationToken = default)
        {
            var user = await _repository.GetUser(request.Email);
            if (user == null || !string.Equals(user.Password, request.Password)) 
                return null;
            var roles = await _repository.GetUserRoles(user.Id);
            var token = await _login.Login(user, roles);

            var result = new LoginResponseModel(user, token);
            result.RoleId = roles.FirstOrDefault();
            return result;
        }
    }
}
