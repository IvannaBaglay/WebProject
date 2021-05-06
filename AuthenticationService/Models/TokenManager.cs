using System;
using System.Collections.Generic;

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace AuthenticationService.Models
{
    public class Token
    {
        public string token { get; set; }
        public int timeToTokenExpire { get; set; }
    }

    public class TokenManager
    {
        public static Token GenerateToken(User user)
        {
            List<Claim> userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.login),
                new Claim(ClaimTypes.Role, user.role)
            };

            JwtSecurityToken token = new JwtSecurityToken(
                    issuer: TokenIssuer,
                    audience: TokenAudience,
                    notBefore: DateTime.UtcNow,
                    claims: userClaims,
                    expires: DateTime.UtcNow.AddMinutes(TokenLifetime),
                    signingCredentials: new SigningCredentials(GetSymetricKey(), SecurityAlgorithms.HmacSha256)
                );

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            return new Token { token = handler.WriteToken(token), timeToTokenExpire = TokenLifetime};
        }

        public static SymmetricSecurityKey GetSymetricKey() => new SymmetricSecurityKey(Secret);

        public static byte[] Secret = new HMACSHA256().Key;
        public const int TokenLifetime = 10;
        public const string TokenIssuer = "IAuthenticationService";
        public const string TokenAudience = "IClient";
    }
}
