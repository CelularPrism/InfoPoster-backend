using InfoPoster_backend.Models.Account;

namespace InfoPoster_backend.Services.Login
{
    public class LoginService
    {
        private readonly IJWTService _jwtService;
        public LoginService(IJWTService jwtService)
        {
            _jwtService = jwtService;
        }

        public async Task<string> Login(UserModel user, List<Guid> roles)
        {
            var claims = await _jwtService.CreateClaims(user, roles);
            var accessToken = await _jwtService.GenerateJwt(claims);
            var accessTokenJti = _jwtService.GetJti(accessToken);
            _jwtService.SetTokenIntoCookie(accessToken);

            return accessToken;
        }

        public async Task<string> RefreshToken()
        {
            var token = _jwtService.GetCurrentToken();
            var claims = _jwtService.GetClaims(token);
            _jwtService.DeleteTokenFromCookie();

            var newAccessToken = await _jwtService.GenerateJwt(claims);
            var newAccessTokenJti = _jwtService.GetJti(newAccessToken);
            _jwtService.SetTokenIntoCookie(newAccessToken);
            return newAccessToken;
        }

        public void Logout()
        {
            _jwtService.DeleteTokenFromCookie();
        }

        public Guid GetUserId()
        {
            try
            {
                var token = _jwtService.GetCurrentToken();
                var idStr = _jwtService.GetIdentifier(token);
                var id = Guid.Parse(idStr);

                return id;
            }
            catch
            {
                return Guid.Empty;
            }
        }
    }
}
