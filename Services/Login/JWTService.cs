using InfoPoster_backend.Models.Account;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;

namespace InfoPoster_backend.Services.Login
{
    public class JWTService : IJWTService
    {
        private readonly IHttpContextAccessor _accessor;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _privateKey;

        private readonly double lifetimeJwt = 60.0;
        public JWTService(IHttpContextAccessor accessor, IConfiguration configuration)
        {
            _accessor = accessor;
            _issuer = configuration["Protection:JwtIssuer"];
            _audience = configuration["Protection:JwtAudience"];
            _privateKey = configuration["Protection:JwtPrivate"];
        }

        public string GetCurrentToken()
        {
            var authorizationHeader = _accessor.HttpContext.Request.Headers["Authorization"];

            return authorizationHeader == StringValues.Empty
                ? string.Empty
                : authorizationHeader.Single().Split(" ").Last();
        }

        public async Task<string> GenerateJwt(List<Claim> claims)
        {
            using RSA rsa = RSA.Create();
            rsa.ImportRSAPrivateKey( // Convert the loaded key from base64 to bytes.
                source: Convert.FromBase64String(_privateKey), // Use the private key to sign tokens
                bytesRead: out int _); // Discard the out variable 

            var mainclass = typeof(AsymmetricSignatureProvider)
                       .GetField(nameof(AsymmetricSignatureProvider.DefaultMinimumAsymmetricKeySizeInBitsForSigningMap), BindingFlags.Public | BindingFlags.Static);
            var field = mainclass.GetValue(null) as Dictionary<string, int>;
            if (field != null) field["RS256"] = 1024;

            var credentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
            {
                CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
            };

            var token = new JwtSecurityToken(
                _issuer,
                _audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(lifetimeJwt),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GetJti(string token)
        {
            if (string.IsNullOrEmpty(token) || !(new JwtSecurityTokenHandler().CanReadToken(token)))
                return null;

            var readToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            return readToken.Payload.Jti;
        }

        public string GetIdentifier(string token)
        {
            if (string.IsNullOrEmpty(token) || !(new JwtSecurityTokenHandler().CanReadToken(token)))
                return null;
            var readToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            return readToken.Claims.FirstOrDefault(a => a.Type == "Identifier")?.Value;
        }

        public async Task<List<Claim>> CreateClaims(UserModel user, List<Guid> roles)
        {
            var claims = new List<Claim>
            {
                new Claim("Identifier", user.Id.ToString())
            };

            claims.AddRange(roles.Select(a => new Claim(ClaimTypes.Role, a.ToString())));

            return claims;
        }

        public List<Claim> GetClaims(string token)
        {
            var readToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
            return readToken.Claims.ToList();
        }

        public void SetTokenIntoCookie(string token)
        {
            _accessor.HttpContext.Response.Cookies.Append("AccessToken", token, new CookieOptions { MaxAge = TimeSpan.FromMinutes(lifetimeJwt) });
        }

        public void DeleteTokenFromCookie()
        {
            _accessor.HttpContext.Response.Cookies.Delete("AccessToken");
        }
    }
}
