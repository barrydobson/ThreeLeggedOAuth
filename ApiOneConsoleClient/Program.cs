using System;
using System.Threading.Tasks;
using ApiOneConsoleClient.Commands;
using static System.Console;

namespace ApiOneConsoleClient
{
    public class Program
    {
        public static void Main(string[] args) => Run(args).GetAwaiter().GetResult();
        private const string ReadPrompt = "client > ";

        public static string ReadFromConsole()
        {
            Write(ReadPrompt);
            return ReadLine();
        }

        public static void WriteToConsole(string message = "")
        {
            if (!string.IsNullOrEmpty(message)) WriteLine(message);
        }

        private static CommandFactory RegisterCommands()
        {
            return new CommandFactory()
                .RegisterCommand(new SetContextCommand())
                .RegisterCommand(new SignInCommand())
                .RegisterCommand(new CallApiCommand())
                .RegisterCommand(new LogoutCommand())
                .RegisterCommand(new ShowContextCommand());
        }

        private static async Task Run(string[] args)
        {
            WriteToConsole(@"Client Application Shell");
            WriteToConsole(@"Starting...");

            var context = new Context();

            var fac = RegisterCommands();

            Clear();

            WriteToConsole(fac.ExecuteHelp());

            if (args.Length == 4 && args[0] == "set-context")
            {
                WriteToConsole("\n\nUsing init args to set context\n\n");
                await Execute(fac, string.Join(" ", args), context);
            }

            while (true)
            {
                var consoleInput = ReadFromConsole();

                if (string.IsNullOrWhiteSpace(consoleInput)) continue;

                var shellCommand = fac.ParseShellCommand(consoleInput);

                if (shellCommand == CommandFactory.ShellCommands.Clear)
                {
                    Clear();
                    continue;
                }

                if (shellCommand == CommandFactory.ShellCommands.Quit) break;

                if (shellCommand == CommandFactory.ShellCommands.Help)
                {
                    WriteToConsole(fac.ExecuteHelp());
                    continue;
                }

                await Execute(fac, consoleInput, context);
            }
        }

        private static async Task Execute(CommandFactory fac, string consoleInput, Context context)
        {
            try
            {
                var result = await fac.ExecuteCommand(consoleInput, context);
                WriteToConsole(result);
            }
            catch (Exception e)
            {
                WriteLine("");
                WriteToConsole("----Error----");
                WriteToConsole(e.Message);
                WriteLine("");
                WriteToConsole(e.StackTrace);
            }
        }
    }
}
