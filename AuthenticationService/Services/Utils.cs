using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Claims;

namespace AuthenticationService.Services
{
    public static class Utils
    {
        public static string GetLoginFromClaims(IEnumerable<Claim> claims) => claims.Where(claim => claim.Type == ClaimTypes.Name).FirstOrDefault()?.Value;
        public static string GetUserRoleFromClaims(IEnumerable<Claim> claims) => claims.Where(claim => claim.Type == ClaimTypes.Role).FirstOrDefault()?.Value;
        public static (string, string) GetLoginPasswordFromAuthHeader(string authHeaderValue)
        {
            string encodedUsernamePassword = authHeaderValue.Substring("Basic ".Length).Trim();

            var encoding = Encoding.GetEncoding("iso-8859-1");
            string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

            int separatorIndex = usernamePassword.IndexOf(':');
            string login = usernamePassword.Substring(0, separatorIndex);
            string password = usernamePassword.Substring(separatorIndex + 1);

            return (login, password);
        }
        public static string LoginPasswordToBasicAuthHeader(string login, string password)
        {
            string loginpassword = $"{login}:{password}";

            var plainTextBytes = Encoding.UTF8.GetBytes(loginpassword);

            string encodedUsernamePassword = Convert.ToBase64String(plainTextBytes);

            return $"Basic {encodedUsernamePassword}";
        }
    }
}
