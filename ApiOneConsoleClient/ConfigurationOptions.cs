using System;
using System.Net.Http;
using IdentityModel.OidcClient;

namespace ApiOneConsoleClient
{
    public class ConfigurationOptions
    {
        public string Authority { get; private set; }
        public string RedirectUrl => DefaultLocalRedirectUrl;
        public HttpClient ApiOneHttpClient { get; private set; }
        public HttpClient AuthApiHttpClient { get; private set; }
        public OidcClientOptions OidcClientOptions { get; private set; }

        private const string DefaultScope = "apione-full openid";
        private const string DefaultLocalRedirectUrl = "http://127.0.0.1:7890/";

        private ConfigurationOptions()
        {

        }

        public static ConfigurationOptions LocalConfiguration(string clientId, OidcClientOptions.AuthenticationFlow flow, string scope = DefaultScope)
        {
            return new ConfigurationOptions
            {
                Authority = Constants.AuthenticationServerUrl,
                ApiOneHttpClient = new HttpClient { BaseAddress = new Uri(Constants.ApiOneUrl) },
                AuthApiHttpClient = new HttpClient { BaseAddress = new Uri(Constants.AuthenticationServerUrl) },
                OidcClientOptions = GetOidcClientOptions(clientId, flow, scope, Constants.AuthenticationServerUrl)
            };
        }

        private static OidcClientOptions GetOidcClientOptions(string clientId, OidcClientOptions.AuthenticationFlow flow, string scope, string authorityUrl)
        {
            return new OidcClientOptions
            {
                Authority = authorityUrl,
                ClientId = clientId,
                RedirectUri = DefaultLocalRedirectUrl,
                Scope = scope,
                Flow = flow
            };
        }
    }


}