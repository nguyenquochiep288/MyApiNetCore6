using DatabaseTHP.StoredProcedure.Parameter;
using MessagePack.Formatters;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace MyApiNetCore6.Class
{
    public class Security
    {
        private readonly IConfiguration _configuration;
        public Security(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<bool> ValidateToken(string authToken)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = await GetValidationParameters();

            SecurityToken validatedToken;
            IPrincipal principal = tokenHandler.ValidateToken(authToken, validationParameters, out validatedToken);
            Thread.CurrentPrincipal = principal;
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            var name = identity.Claims.Where(c => c.Type == ClaimTypes.Name)
                   .Select(c => c.Value).SingleOrDefault();

            var tokenExp = identity.Claims.First(claim => claim.Type.Equals("exp")).Value;
            var tokenDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(tokenExp)).UtcDateTime;
            var now = DateTime.Now.ToUniversalTime();
            return true;
        }

        public async Task<string> GetUserName(string authToken)
        {

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = await GetValidationParameters();

            SecurityToken validatedToken;
            IPrincipal principal = tokenHandler.ValidateToken(authToken, validationParameters, out validatedToken);
            Thread.CurrentPrincipal = principal;
            var identity = (ClaimsPrincipal)Thread.CurrentPrincipal;
            var name = identity.Claims.Where(c => c.Type == ClaimTypes.Name)
                   .Select(c => c.Value).SingleOrDefault();
            return name ?? "";
        }

        public async Task<TokenValidationParameters> GetValidationParameters()
        {
            return new TokenValidationParameters()
            {
                ValidateLifetime = true, // Because there is no expiration in the generated token
                ValidateAudience = false, // Because there is no audiance in the generated token
                ValidateIssuer = false,   // Because there is no issuer in the generated token
                ValidIssuer =  _configuration["JWT:ValidIssuer"],
                ValidAudience = _configuration["JWT:ValidAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]))// The same key as the one that generate the token
            };
        }
    }
}
