using Microsoft.AspNetCore.Authorization;

namespace ApiTwo
{
    public static class Policy
    {
        public const string ApiTwoUserPolicy = "ApiTwoUser";
    }

    public static class PolicyExtensions
    {
        public static void AddApiTwoPolicy(this AuthorizationOptions options)
        {
            options.AddPolicy(Policy.ApiTwoUserPolicy, policy => policy.RequireClaim("scope", "apitwo-ro"));
        }
    }
}