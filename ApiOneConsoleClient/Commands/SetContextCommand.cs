using System;
using System.Threading.Tasks;
using IdentityModel.OidcClient;

namespace ApiOneConsoleClient.Commands
{
    public class SetContextCommand : ICommand
    {
        private enum Action
        {
            Invalid,
            Local
        }

        public Task<string> Execute(Context context, params string[] args)
        {
            var action = GetAction(args[0]);
            var client = args[1];
            var flow = GetFlow(args[2]);

            if (action == Action.Invalid)
                return Task.FromResult($"Invalid action {args[0]}");

            switch (action)
            {
                case Action.Local:
                    context.SetConfiguration(ConfigurationOptions.LocalConfiguration(client, flow));
                    break;
                default:
                    throw new ApplicationException("Unkown Action");
            }

            return Task.FromResult(
                $"Set Context to::\n    environment: {action.ToString()}\n    client: {client}\n    flow: {flow.ToString()}\n");
        }

        private static Action GetAction(string param)
        {
            switch (param.ToLower())
            {
                case "local":
                    return Action.Local;
                default:
                    return Action.Invalid;
            }
        }

        private static OidcClientOptions.AuthenticationFlow GetFlow(string flow)
        {
            switch (flow.ToLower())
            {
                case "hybrid":
                    return OidcClientOptions.AuthenticationFlow.Hybrid;
                case "code":
                    return OidcClientOptions.AuthenticationFlow.AuthorizationCode;
                default:
                    throw new ArgumentOutOfRangeException(nameof(flow), "Invlalid flow. hybrid or code are allowed");
            }
        }

        public string CommandString => "set-context";
        public string HelpString => "set-context <local> <clientId> <hybrid/code>\n    Set the name of the context";
        public int NumberOfArgs => 3;
    }
}