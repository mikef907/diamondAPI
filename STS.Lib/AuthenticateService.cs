using Common.Lib;
using Common.Lib.Models.DM;
using Common.Lib.ServiceAgent;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace STS.Lib
{
    public interface IAuthenticateService
    {
        Task<AccessTokenResponse> GrantAccessToken(AccessTokenRequest model);
        Task<AccessTokenResponse> ConsumeRefreshToken(RefreshTokenRequest model);
    }

    public class AuthenticateService : IAuthenticateService
    {
        private AppSettings _appSettings;

        private IIdentityAgent _identityAgent;
        private JwtSecurityTokenHandler _tokenHandler { get; set; } = new JwtSecurityTokenHandler();
        private static SecurityToken _stsToken { get; set; }
        public AuthenticateService(IOptions<AppSettings> appSettings, IIdentityAgent identityAgent)
        {
            _appSettings = appSettings.Value;
            _identityAgent = identityAgent;
            if (_stsToken == null || _stsToken.ValidTo > DateTime.Now)
            {
                CreateSTSToken();
            }
        }

        public async Task<AccessTokenResponse> GrantAccessToken(AccessTokenRequest model)
        {
            var authModel = new AuthenticateModel()
            {
                Email = model.Username,
                Password = model.Password
            };

            Guid? userId = await _identityAgent.Authenticate(authModel, _stsToken);

            if (userId.HasValue)
            {
                var token = CreateUserToken(userId.Value);
                var refreshToken = GenerateRefreshToken(token as JwtSecurityToken);

                await _identityAgent.CreateRefreshToken(refreshToken, _stsToken);

                return new AccessTokenResponse()
                {
                    Token_Type = "bearer",
                    Access_Token = _tokenHandler.WriteToken(token),
                    Refresh_Token = refreshToken.Token
                };
            }
            else
            {
                return null;

            }
        }

        public async Task<AccessTokenResponse> ConsumeRefreshToken(RefreshTokenRequest model) {
            var claimsPrincipal = GetPrincipalFromExpiredToken(model.Client_Secret);

            Guid userId = Guid.Parse(claimsPrincipal.Identity.Name);
            Guid jti = Guid.Parse(claimsPrincipal.Claims.Single(c => c.Type == JwtRegisteredClaimNames.Jti).Value);

            var currentRefreshToken = await _identityAgent.FetchRefreshToken(userId, jti, _stsToken);

            // TODO: Explore just not returning a token from identity if some of these conditions are not true?
            if (currentRefreshToken != null && currentRefreshToken.Valid && currentRefreshToken.InUse 
                && currentRefreshToken.Expiry > DateTime.UtcNow && currentRefreshToken.Token == model.Refresh_Token)
            {
                var newToken = CreateUserToken(userId);
                var newRefreshToken = GenerateRefreshToken(newToken as JwtSecurityToken);

                await _identityAgent.RemoveRefreshToken(model.Refresh_Token, _stsToken);
                await _identityAgent.CreateRefreshToken(newRefreshToken, _stsToken);

                return new AccessTokenResponse() 
                { 
                    Token_Type = "bearer",
                    Access_Token = _tokenHandler.WriteToken(newToken), 
                    Refresh_Token = newRefreshToken.Token 
                };
            }
            else {
                return null;
            }
        }

        private void CreateSTSToken()
        {
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            _stsToken = _tokenHandler.CreateToken(new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                    {
                            new Claim("STS", "true")
                    }),
                Expires = DateTime.UtcNow.AddDays(1),
                NotBefore = DateTime.UtcNow,
                Issuer = "STS",
                Audience = "Token Service",
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            });

        }

        private SecurityToken CreateUserToken(Guid userId)
        {
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, userId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Issuer = "STS",
                Audience = "Users",
                Expires = DateTime.UtcNow.AddSeconds(30),
                NotBefore = DateTime.UtcNow,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            return _tokenHandler.CreateToken(tokenDescriptor);
        }

        private RefreshToken GenerateRefreshToken(JwtSecurityToken token)
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);

                // TODO: Add Client IP
                return new RefreshToken()
                {
                    Token = Convert.ToBase64String(randomNumber),
                    JwtId = Guid.Parse(token.Id),
                    UserId = Guid.Parse(token.Claims.Single(c => c.Type == JwtRegisteredClaimNames.UniqueName).Value),
                    Valid = true,
                    InUse = true,
                    Expiry = DateTime.UtcNow.AddDays(10),
                };
            }
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_appSettings.Secret)),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidAudiences = new[] { "Users" },
                ValidIssuer = "STS",
                ValidateLifetime = false,
                RequireExpirationTime = true
            };

            SecurityToken securityToken;
            var principal = _tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
    }
}
