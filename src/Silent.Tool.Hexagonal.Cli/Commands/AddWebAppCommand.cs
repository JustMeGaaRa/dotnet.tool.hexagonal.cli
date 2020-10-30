using CliFx;
using CliFx.Attributes;
using Microsoft.Extensions.Options;
using Silent.Tool.Hexagonal.Cli.Infrastructure.Options;
using System;
using System.Threading.Tasks;

namespace Silent.Tool.Hexagonal.Cli
{
    [Command("add webapp")]
    public class AddWebAppCommand : ICommand
    {
        private readonly IOptions<GeneralOptions> _options;

        public AddWebAppCommand(IOptions<GeneralOptions> options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        [CommandParameter(0)]
        public string WebAppName { get; set; }

        [CommandOption("--framework", 'f')]
        public string Framework { get; set; }

        public ValueTask ExecuteAsync(IConsole console)
        {
            var task = Task.FromResult(HandleAddWebAppCommand(_options));
            return new ValueTask(task);
        }

        private bool HandleAddWebAppCommand(IOptions<GeneralOptions> options)
        {
            string frameworkOption = Framework ?? options.Value.Framework.Default;
            string serviceRelativePath = $"src/{options.Value.Folders.WebAppsFolder}";
            string webappProjectName = $"{WebAppName}.Web";
            string webappRelativePath = $"{serviceRelativePath}/{webappProjectName}";
            string webappCommand = $"dotnet new webapp --name \"{webappProjectName}\" --output \"{webappRelativePath}\" --framework {frameworkOption}";

            string solutionWebAppReferences = $"dotnet sln add"
                + $" \"{webappRelativePath}/{webappProjectName}.csproj\""
                + $" --solution-folder \"src\"";

            var successfull = true;

            successfull = successfull && StringExtensions.RunInCommandPrompt(webappCommand);
            successfull = successfull && StringExtensions.RunInCommandPrompt(solutionWebAppReferences);

            return successfull;
        }
    }
}
