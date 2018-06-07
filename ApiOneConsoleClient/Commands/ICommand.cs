using System.Threading.Tasks;

namespace ApiOneConsoleClient.Commands
{
    public interface ICommand
    {
        Task<string> Execute(Context context, params string[] args);
        string CommandString { get; }
        string HelpString { get; }
        int NumberOfArgs { get; }
    }
}