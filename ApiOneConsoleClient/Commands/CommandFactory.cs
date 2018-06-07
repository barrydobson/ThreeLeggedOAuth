using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ApiOneConsoleClient.Commands
{
    public class CommandFactory
    {
        private readonly List<ICommand> _commands = new List<ICommand>();

        public enum ShellCommands
        {
            None,
            Help,
            Quit,
            Clear
        }

        public ShellCommands ParseShellCommand(string input)
        {
            switch (input.ToLower())
            {
                case "help":
                    return ShellCommands.Help;
                case "quit":
                case "exit":
                    return ShellCommands.Quit;
                case "clear":
                    return ShellCommands.Clear;
                default:
                    return ShellCommands.None;
            }
        }

        public CommandFactory RegisterCommand(ICommand command)
        {
            if (command.CommandString.Contains(" "))
                throw new ArgumentException("Command Strings must be a single word");

            if (_commands.Exists(x => x.CommandString == command.CommandString))
                throw new ArgumentException($"Command already registred with command string {command.CommandString}");

            _commands.Add(command);
            return this;
        }

        public async Task<string> ExecuteCommand(string input, Context context)
        {
            var command = _commands.SingleOrDefault(x => input.ToLower().StartsWith(x.CommandString));

            if (command == null)
                throw new ArgumentException("No command found");

            var args = Regex.Split(input, "(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*) (?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)")
                .TakeLast(command.NumberOfArgs).ToArray();
            return await command.Execute(context, args);
        }

        public string ExecuteHelp() => GetHelp();

        private string GetHelp()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Commands are..");

            foreach (var command in _commands)
            {
                sb.AppendLine($@"  {command.HelpString}");
            }

            sb.AppendLine("  clear\n     Clears the console");
            sb.AppendLine("  help\n      Shows this message");
            sb.AppendLine("  quit\n      Exits the shell");

            return sb.ToString();
        }
    }
}