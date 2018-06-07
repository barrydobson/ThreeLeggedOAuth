using System.Threading.Tasks;

namespace ApiOneConsoleClient.Commands
{
    public class ShowContextCommand : ICommand
    {
        public Task<string> Execute(Context context, params string[] args)
        {
            return Task.FromResult(context.ToString());
        }

        public string CommandString => "show-context";
        public string HelpString => "show-context\n    Shows the values stored in context";
        public int NumberOfArgs => 0;
    }
}