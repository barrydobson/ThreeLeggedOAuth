using System.Linq;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiTwo.Controllers
{
    [Route("identity")]
    public class IdentityController : ControllerBase
    {
        [HttpGet]
        [Authorize(Policy = Policy.ApiTwoUserPolicy, AuthenticationSchemes = IdentityServerAuthenticationDefaults.AuthenticationScheme)]
        public IActionResult Get()
        {
            var claimsArray = from c in User.Claims select new { c.Type, c.Value };
            return new JsonResult(new { claimsApiTwo = claimsArray });
        }
    }
}
