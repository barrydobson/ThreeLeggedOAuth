using System.Threading.Tasks;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using Microsoft.Net.Http.Server;

namespace ApiOneConsoleClient.Commands
{
    public class LogoutCommand : ICommand
    {
        public async Task<string> Execute(Context context, params string[] args)
        {
            if (!context.IsSet)
                return "Context must be set first";

            if (!context.IsSignedIn)
                return "User must be signed in first";

            var logoutUrl = await context.OidcClient.PrepareLogoutAsync(new LogoutRequest { IdTokenHint = context.IdToken, BrowserDisplayMode = DisplayMode.Hidden });
            context.OpenBrowser(logoutUrl);

            RequestContext webcontext;
            while (true)
            {
                webcontext = await context.WebListener.AcceptAsync();
                break;
            }
            await context.SendResponseAsync(webcontext.Response, "Hi");

            context.IdToken = string.Empty;
            context.AccessToken = string.Empty;
            context.RefreshToken = string.Empty;

            return "Logged out";
        }

        public string CommandString => "logout";
        public string HelpString => "logout\n    Logout of the authentication server";
        public int NumberOfArgs => 0;
    }
}