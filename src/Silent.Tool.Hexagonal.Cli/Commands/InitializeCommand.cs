using CliFx;
using CliFx.Attributes;
using CliWrap;
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

        public async ValueTask ExecuteAsync(IConsole console)
        {
            await HandleIniCommand(_options);
        }

        private async Task<bool> HandleIniCommand(IOptions<GeneralOptions> options)
        {
            var consoleOutputTarget = PipeTarget.ToStream(Console.OpenStandardOutput());

            // dotnet new sln --name \"{ProjectName}\" --output \"{ProjectName}\"
            var domainCommand = CliWrap.Cli.Wrap("dotnet")
                .WithStandardOutputPipe(consoleOutputTarget)
                .WithArguments(args => args
                    .Add("new")
                    .Add("sln")
                    .Add("--name")
                    .Add(ProjectName)
                    .Add("--output")
                    .Add(ProjectName));

            var domainCommandResult = await domainCommand.ExecuteAsync();

            var successfull = true;

            successfull &= StringExtensions.CreateDirectorySafely($"{ProjectName}/{options.Value.Folders.ClientsFolder}");
            successfull &= StringExtensions.CreateDirectorySafely($"{ProjectName}/{options.Value.Folders.DocsFolder}");
            successfull &= StringExtensions.CreateDirectorySafely($"{ProjectName}/{options.Value.Folders.SamplesFolder}");
            successfull &= StringExtensions.CreateDirectorySafely($"{ProjectName}/src/{options.Value.Folders.ServicesFolder}");
            successfull &= StringExtensions.CreateDirectorySafely($"{ProjectName}/src/{options.Value.Folders.WebAppsFolder}");
            successfull &= StringExtensions.CreateDirectorySafely($"{ProjectName}/test");
            successfull &= domainCommandResult.ExitCode == 0;

            return successfull;
        }
    }
}
