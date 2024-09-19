using InfoPoster_backend.Models.Account;
using System.Security.Claims;

namespace InfoPoster_backend.Services.Login
{
    public interface IJWTService
    {
        string GetCurrentToken();
        Task<string> GenerateJwt(List<Claim> claims);
        string GetJti(string token);
        string GetIdentifier(string token);
        Task<List<Claim>> CreateClaims(UserModel user, List<Guid> roles);
        List<Claim> GetClaims(string token);
        void SetTokenIntoCookie(string token);
        void DeleteTokenFromCookie();
    }
}
