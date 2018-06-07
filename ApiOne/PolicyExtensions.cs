using Microsoft.AspNetCore.Authorization;

namespace ApiOne
{
    public static class Policy
    {
        public const string ApiOneUserPolicy = "ApiOneUser";
    }

    public static class PolicyExtensions
    {
        public static void AddApiOnePolicy(this AuthorizationOptions options)
        {
            options.AddPolicy(Policy.ApiOneUserPolicy, policy => policy.RequireClaim("scope", "apione-full"));
        }
    }
}