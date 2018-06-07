using System.IO;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.OidcClient;
using Microsoft.Net.Http.Server;

namespace ApiOneConsoleClient.Commands
{
    public class SignInCommand : ICommand
    {

        public async Task<string> Execute(Context context, params string[] args)
        {
            if (!context.IsSet)
                return "Context must be set first";

            var state = await context.OidcClient.PrepareLoginAsync();

            context.OpenBrowser(state.StartUrl);

            var webcontext = await context.WebListener.AcceptAsync();
            var formData = GetRequestPostData(webcontext.Request);

            var sb = new StringBuilder();

            if (formData == null)
            {
                sb.AppendLine("Invalid response from authenticate call back");
                return null;
            }

            await context.SendResponseAsync(webcontext.Response, "<html><head></head><body>Logged in!!... Please return to the app.</body></html>");

            var result = await context.OidcClient.ProcessResponseAsync(formData, state);

            sb.AppendLine(ShowResult(result));

            context.AccessToken = result.AccessToken;
            context.RefreshToken = result.RefreshToken;
            context.IdToken = result.IdentityToken;

            return sb.ToString();
        }

        private static string ShowResult(LoginResult result)
        {
            if (result.IsError)
                return $"\n\nError:\n{result.Error}";

            var sb = new StringBuilder();
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("Claims:");

            foreach (var claim in result.User.Claims)
            {
                sb.AppendLine($"{claim.Type}: {claim.Value}");
            }

            sb.AppendLine("");
            sb.AppendLine($"identity token: {result.IdentityToken}");
            sb.AppendLine($"access token:   {result.AccessToken}");
            sb.AppendLine($"refresh token:  {result.RefreshToken ?? "none"}");

            return sb.ToString();
        }

        private static string GetRequestPostData(Request request)
        {
            if (!request.HasEntityBody)
            {
                return null;
            }

            using (var body = request.Body)
            {
                using (var reader = new StreamReader(body))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public string CommandString => "signin";
        public string HelpString => "signin\n    Initiate sing in process";
        public int NumberOfArgs => 0;
    }
}