using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ApiOneConsoleClient.Commands
{
    public class CallApiCommand : ICommand
    {
        public async Task<string> Execute(Context context, params string[] args)
        {
            if (!context.IsSet)
                return "Context must be set first";

            if (!context.IsSignedIn)
                return "User must be signed in first";

            var path = args[0];
            context.Configuration.ApiOneHttpClient.SetBearerToken(context.AccessToken);
            var response = await context.Configuration.ApiOneHttpClient.GetAsync(path);
            var sb = new StringBuilder();

            if (response.IsSuccessStatusCode)
            {
                var s = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(s);
                sb.AppendLine("");
                sb.AppendLine("");
                sb.AppendLine(json.ToString(Formatting.Indented));
            }
            else
            {
                sb.AppendLine($"Error: {response.ReasonPhrase}");
            }

            return sb.ToString();
        }

        public string CommandString => "api";

        public string HelpString =>
            "api <path>\n    Call the API. Just supply the path, the base url is set in context";
        public int NumberOfArgs => 1;
    }
}