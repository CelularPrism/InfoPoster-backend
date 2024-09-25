using InfoPoster_backend.Services.Login;
using MediatR;

namespace InfoPoster_backend.Handlers.Account
{
    public class LogoutRequest : IRequest<LogoutResponse> { }

    public class LogoutResponse { }

    public class LogoutHandler : IRequestHandler<LogoutRequest, LogoutResponse>
    {
        private readonly LoginService _login;

        public LogoutHandler(LoginService login)
        {
            _login = login;
        }

        public async Task<LogoutResponse> Handle(LogoutRequest request, CancellationToken cancellationToken)
        {
            _login.Logout();
            return new LogoutResponse();
        }
    }
}
