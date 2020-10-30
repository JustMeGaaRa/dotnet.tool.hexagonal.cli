using CliFx;
using CliFx.Attributes;
using Microsoft.Extensions.Options;
using Silent.Tool.Hexagonal.Cli.Infrastructure.Options;
using System;
using System.Threading.Tasks;

namespace Silent.Tool.Hexagonal.Cli
{
    [Command("add service")]
    public class AddServiceCommand : ICommand
    {
        private readonly IOptions<GeneralOptions> _options;

        public AddServiceCommand(IOptions<GeneralOptions> options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        [CommandParameter(0)]
        public string ServiceName { get; set; }

        [CommandOption("--framework", 'f')]
        public string Framework { get; set; }

        public ValueTask ExecuteAsync(IConsole console)
        {
            var task = Task.FromResult(HandleAddServiceCommand(_options));
            return new ValueTask(task);
        }

        private bool HandleAddServiceCommand(IOptions<GeneralOptions> options)
        {
            string framework = Framework ?? options.Value.Framework.Default;
            string serviceRelativePath = $"src/{options.Value.Folders.ServicesFolder}/{ServiceName}";

            string domainProjectName = $"{ServiceName}.Domain";
            string infraProjectName = $"{ServiceName}.Infrastructure";
            string webapiProjectName = $"{ServiceName}.Api";
            string testProjectName = $"{ServiceName}.Domain.Tests";

            string domainRelativePath = $"{serviceRelativePath}/{domainProjectName}";
            string infraRelativePath = $"{serviceRelativePath}/{infraProjectName}";
            string webapiRelativePath = $"{serviceRelativePath}/{webapiProjectName}";
            string testRelativePath = $"test/{testProjectName}";

            string domainCommand = $"dotnet new classlib --name \"{domainProjectName}\" --output \"{domainRelativePath}\" --framework {framework}";
            string infraCommand = $"dotnet new classlib --name \"{infraProjectName}\" --output \"{infraRelativePath}\" --framework {framework}";
            string webapiCommand = $"dotnet new webapi --name \"{webapiProjectName}\" --output \"{webapiRelativePath}\" --framework {framework}";
            string unitTestCommand = $"dotnet new xunit --name \"{testProjectName}\" --output \"{testRelativePath}\" --framework {framework}";

            var successfull = true;
            successfull = successfull && StringExtensions.RunInCommandPrompt(domainCommand);
            successfull = successfull && StringExtensions.RunInCommandPrompt(infraCommand);
            successfull = successfull && StringExtensions.RunInCommandPrompt(webapiCommand);
            successfull = successfull && StringExtensions.RunInCommandPrompt(unitTestCommand);

            return successfull;
        }
    }
}
