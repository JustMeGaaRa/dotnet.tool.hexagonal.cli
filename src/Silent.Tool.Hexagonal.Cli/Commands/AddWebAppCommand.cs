using CliFx;
using CliFx.Attributes;
using CliWrap;
using Microsoft.Extensions.Options;
using Silent.Tool.Hexagonal.Cli.Infrastructure.Options;
using System;
using System.Linq;
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

        public string WebAppProjectName => $"{WebAppName}.Web";

        public async ValueTask ExecuteAsync(IConsole console)
        {
            await HandleProjectCreation(_options);
            await HandleProjectReferences(_options);
            await HandleSolutionReferences(_options);
        }

        private string GetServiceRelativePath(IOptions<GeneralOptions> options, string webappProjectName)
        {
            string serviceRelativePath = $"src/{options.Value.Folders.WebAppsFolder}/{WebAppName}/{webappProjectName}";
            return serviceRelativePath;
        }

        private async Task<bool> HandleProjectCreation(IOptions<GeneralOptions> options)
        {
            var consoleOutputTarget = PipeTarget.ToStream(Console.OpenStandardOutput());
            string framework = Framework ?? options.Value.Framework.Default;
            string webappRelativePath = GetServiceRelativePath(options, WebAppProjectName);

            // dotnet new webapp --name \"{WebAppProjectName}\" --output \"{webappRelativePath}\" --framework {framework}
            var webappCommand = CliWrap.Cli.Wrap("dotnet")
                .WithStandardOutputPipe(consoleOutputTarget)
                .WithArguments(args => args
                    .Add("new")
                    .Add("webapp")
                    .Add("--name")
                    .Add(WebAppProjectName)
                    .Add("--output")
                    .Add(webappRelativePath)
                    .Add("--framework")
                    .Add(framework));

            var webappResult = await webappCommand.ExecuteAsync();

            var results = new[]
            {
                webappResult
            };

            return results.All(r => r.ExitCode == 0);
        }

        private Task<bool> HandleProjectReferences(IOptions<GeneralOptions> options)
        {
            return Task.FromResult(true);
        }

        private async Task<bool> HandleSolutionReferences(IOptions<GeneralOptions> options)
        {
            var consoleOutputTarget = PipeTarget.ToStream(Console.OpenStandardOutput());
            string webappRelativePath = GetServiceRelativePath(options, WebAppProjectName);

            var solutionServiceReferencesCommand = CliWrap.Cli.Wrap("dotnet")
                .WithStandardOutputPipe(consoleOutputTarget)
                .WithArguments(args => args
                    .Add("sln")
                    .Add("add")
                    .Add($"{webappRelativePath}/{WebAppProjectName}.csproj")
                    .Add("--solution-folder")
                    .Add("src"));

            var solutionServiceReferencesResult = await solutionServiceReferencesCommand.ExecuteAsync();

            var results = new[]
            {
                solutionServiceReferencesResult
            };

            return results.All(r => r.ExitCode == 0);
        }
    }
}
