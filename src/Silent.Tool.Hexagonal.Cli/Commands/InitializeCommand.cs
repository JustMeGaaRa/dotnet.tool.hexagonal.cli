using CliFx;
using CliFx.Attributes;
using Microsoft.Extensions.Options;
using Silent.Tool.Hexagonal.Cli.Infrastructure.Options;
using System;
using System.Threading.Tasks;

namespace Silent.Tool.Hexagonal.Cli
{
    [Command("init")]
    public class InitializeCommand : ICommand
    {
        private readonly IOptions<GeneralOptions> _options;

        public InitializeCommand(IOptions<GeneralOptions> options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        [CommandParameter(0)]
        public string ProjectName { get; set; }

        public ValueTask ExecuteAsync(IConsole console)
        {
            var task = Task.FromResult(HandleIniCommand(_options));
            return new ValueTask(task);
        }

        private bool HandleIniCommand(IOptions<GeneralOptions> options)
        {
            string solutionCommand = $"dotnet new sln --name \"{ProjectName}\" --output \"{ProjectName}\"";

            var successfull = true;
            successfull &= StringExtensions.CreateDirectorySafely($"{ProjectName}/{options.Value.Folders.ClientsFolder}");
            successfull &= StringExtensions.CreateDirectorySafely($"{ProjectName}/{options.Value.Folders.DocsFolder}");
            successfull &= StringExtensions.CreateDirectorySafely($"{ProjectName}/{options.Value.Folders.SamplesFolder}");
            successfull &= StringExtensions.CreateDirectorySafely($"{ProjectName}/src/{options.Value.Folders.ServicesFolder}");
            successfull &= StringExtensions.CreateDirectorySafely($"{ProjectName}/src/{options.Value.Folders.WebAppsFolder}");
            successfull &= StringExtensions.CreateDirectorySafely($"{ProjectName}/test");
            successfull &= StringExtensions.RunInCommandPrompt(solutionCommand);

            return successfull;
        }
    }
}
