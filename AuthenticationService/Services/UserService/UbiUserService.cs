using AuthenticationService.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationService.Services.UserService
{
    public class UbiUserService : IUserService
    {
        readonly Uri UbiServicesUri;
        public UbiUserService(IConfiguration configuration)
        {
            UbiServicesUri = new Uri(configuration.GetConnectionString("Ubiservices"));
        }

        public async Task<(bool, Token)> Login(string login, string password)
        {
            string basicAuthHeader = Utils.LoginPasswordToBasicAuthHeader(login, password);

            HttpRequestMessage msq = new HttpRequestMessage();

            msq.RequestUri = UbiServicesUri;
            msq.Method = HttpMethod.Post;
            msq.Content = new StringContent("{}", Encoding.UTF8, "application/json");
            msq.Headers.TryAddWithoutValidation("Ubi-AppId", "437edb3b-560f-4a6c-b921-407732971297");
            msq.Headers.TryAddWithoutValidation("Authorization", basicAuthHeader);

            var result = await new HttpClient().SendAsync(msq);

            string strResponse = await result.Content.ReadAsStringAsync();
            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                // TO LOG: throw new HttpRequestException($"Request error.\n{strResponse}");
                return Fail();
            }

            UbiServiceUser jUser = JsonConvert.DeserializeObject<UbiServiceUser>(strResponse);
            Token token = new Token { token = jUser.ticket, timeToTokenExpire = jUser.TokenExpirationTime() };
            return Success(token);

        }
        public async Task<(bool, Token)> Refresh(IEnumerable<Claim> claims)
        {
            string authHeaderValue = claims.Where(claim => claim.Type == "access_token").FirstOrDefault().Value;

            HttpRequestMessage msq = new HttpRequestMessage();

            msq.RequestUri = UbiServicesUri;
            msq.Method = HttpMethod.Put;

            msq.Content = new StringContent("{}", Encoding.UTF8, "application/json");
            msq.Headers.TryAddWithoutValidation("Ubi-AppId", "437edb3b-560f-4a6c-b921-407732971297");
            msq.Headers.TryAddWithoutValidation("Authorization", $"ubi_v1 t={authHeaderValue}");

            var result = await new HttpClient().SendAsync(msq);

            string strResponse = await result.Content.ReadAsStringAsync();
            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                // TO LOG: throw new HttpRequestException($"Request error.\n{strResponse}");
                return Fail();
            }

            UbiServiceUser jUser = JsonConvert.DeserializeObject<UbiServiceUser>(strResponse);

            Token token = new Token { token = jUser.ticket, timeToTokenExpire = jUser.TokenExpirationTime() };
            return Success(token);
        }

        public async Task<bool> CloseSession(IEnumerable<Claim> claims)
        {
            string authHeaderValue = claims.Where(claim => claim.Type == "access_token").FirstOrDefault().Value;

            HttpRequestMessage msq = new HttpRequestMessage();

            msq.RequestUri = UbiServicesUri;
            msq.Method = HttpMethod.Delete;

            msq.Content = new StringContent("{}", Encoding.UTF8, "application/json");
            msq.Headers.TryAddWithoutValidation("Ubi-AppId", "437edb3b-560f-4a6c-b921-407732971297");
            msq.Headers.TryAddWithoutValidation("Authorization", $"ubi_v1 t={authHeaderValue}");

            var result = await new HttpClient().SendAsync(msq);

            return result.StatusCode == System.Net.HttpStatusCode.OK;
        }


        public async Task<(bool, Token)> Register(string login, string password) => Fail();


        public async Task<(bool, Token)> ChangeUser(IEnumerable<Claim> claims, User newUser) => Fail();
        public async Task<(bool, Token)> ChangeUser(int id, User newUser) => Fail();


        public async Task<bool> DeleteUser(IEnumerable<Claim> claims) => false;
        public async Task<bool> DeleteUser(int id) => false;

        protected (bool, Token) Fail() => (false, null);
        protected (bool, Token) Success(Token token) => (true, token);

        public async Task<(bool, List<User>)> GetAllUsers() => (false, null);

        public async Task<(bool, User)> GetUser(int id) => (false, null);
    }
}
