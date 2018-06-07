using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Net.Http.Server;
using Serilog;

namespace ApiOneConsoleClient
{
    public class Context
    {
        public ConfigurationOptions Configuration { get; private set; }
        public string AccessToken { get; set; }
        public string IdToken { get; set; }
        public string RefreshToken { get; set; }
        public bool IsSet { get; private set; }
        public bool IsSignedIn => !string.IsNullOrEmpty(AccessToken);
        public bool HasRefreshToken => !string.IsNullOrEmpty(RefreshToken);
        public IdentityModel.OidcClient.OidcClient OidcClient { get; private set; }
        public WebListener WebListener { get; private set; }

        public void SetConfiguration(ConfigurationOptions config)
        {
            Configuration = config ?? throw new ArgumentNullException(nameof(config));
            IsSet = true;
            Configure();
            StartHttpListener();
        }

        private void Configure()
        {
            var options = Configuration.OidcClientOptions;

            var serilog = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .Enrich.FromLogContext()
                .WriteTo.LiterateConsole(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message}{NewLine}{Exception}{NewLine}")
                .CreateLogger();

            options.LoggerFactory.AddSerilog(serilog);

            OidcClient = new IdentityModel.OidcClient.OidcClient(options);
        }

        private void StartHttpListener()
        {
            var settings = new WebListenerSettings();
            settings.UrlPrefixes.Add(Configuration.RedirectUrl);
            WebListener = new WebListener(settings);

            WebListener.Start();
        }

        public void OpenBrowser(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task SendResponseAsync(Response response, string message)
        {
            var buffer = Encoding.UTF8.GetBytes(message);

            response.ContentLength = buffer.Length;

            var responseOutput = response.Body;
            await responseOutput.WriteAsync(buffer, 0, buffer.Length);
            responseOutput.Flush();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine("");
            sb.AppendLine("** Current Context **");
            sb.AppendLine($"Signed In: {(IsSignedIn ? "yes" : "no")}");
            sb.AppendLine($"AccessToken: {AccessToken}");
            sb.AppendLine($"IdentityToken: {IdToken}");
            sb.AppendLine($"RefreshToken: {RefreshToken}");
            sb.AppendLine($"Authority: {Configuration.Authority}");
            sb.AppendLine($"Api One Url: {Configuration.ApiOneHttpClient.BaseAddress}");
            sb.AppendLine("");
            sb.AppendLine("");

            return sb.ToString();
        }
    }
}