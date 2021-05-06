using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Encodings.Web;
using System.Net.Http.Headers;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using AuthenticationService.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;

namespace AuthenticationService.Services
{
    public class UbiServicesAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        readonly Uri UbiServicesUri;
        public UbiServicesAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IConfiguration configuration)
            : base(options, logger, encoder, clock)
        {
            UbiServicesUri = new Uri(configuration.GetConnectionString("Ubiservices"));
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Missing Authorization Header");
            }

            string authVal = Request.Headers.SingleOrDefault(header => header.Key == "Authorization").Value;

            if (!authVal.Contains("Bearer "))
            {
                return AuthenticateResult.Fail("Invalid Authorization type");
            }

            authVal = authVal.Substring("Bearer ".Length);

            HttpRequestMessage msq = new HttpRequestMessage();

            msq.RequestUri = UbiServicesUri;
            msq.Method = HttpMethod.Post;

            msq.Content = new StringContent("{}", Encoding.UTF8, "application/json");
            msq.Headers.TryAddWithoutValidation("Ubi-AppId", "437edb3b-560f-4a6c-b921-407732971297");
            msq.Headers.TryAddWithoutValidation("Authorization", $"ubi_v1 t={authVal}");

            var result = await new HttpClient().SendAsync(msq);

            string strResponse = await result.Content.ReadAsStringAsync();

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return AuthenticateResult.Fail($"Request error.\n{strResponse}");
            }

            UbiServiceUser jUser = JsonConvert.DeserializeObject<UbiServiceUser>(strResponse);

            var claims = new[] { new Claim(ClaimTypes.Name, jUser.nameOnPlatform),
                                 new Claim(ClaimTypes.Role, "user"),
                                 new Claim("access_token", authVal)};
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }
    }
}
