using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace ApiOne.Controllers
{
    [Route("delegate")]
    public class DelegateController : ControllerBase
    {
        private const string ApiTwoUrl = "https://localhost:43003/identity";
        private const string AuthenticationServerUrl = "https://localhost:44301";

        [HttpGet]
        [Authorize(Policy = Policy.ApiOneUserPolicy, AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Get()
        {
            var response = new
            {
                firstHopClaims = from c in User.Claims select new {c.Type, c.Value},
                secondHopClaims = await CallOtherApi()
            };

            return new JsonResult(response);
        }

        private async Task<JObject> CallOtherApi()
        {
            var userToken = Request.Headers["Authorization"][0].Substring("Bearer ".Length);
            var newToken = await DelegateAsync(userToken);
            using (var httpClient = new HttpClient())
            {
                httpClient.SetBearerToken(newToken.AccessToken);
                var result = await httpClient.GetStringAsync(ApiTwoUrl);
                return JObject.Parse(result);
            }
        }


        public async Task<TokenResponse> DelegateAsync(string userToken)
        {
            var payload = new
            {
                token = userToken
            };

            var discoClient = new DiscoveryClient(AuthenticationServerUrl);
            var disco = await discoClient.GetAsync();

            // create token client
            var client = new TokenClient(disco.TokenEndpoint, "apione", "secret");

            // send custom grant to token endpoint, return response
            return await client.RequestCustomGrantAsync("delegation", "apitwo-ro", payload);
        }
    }
}