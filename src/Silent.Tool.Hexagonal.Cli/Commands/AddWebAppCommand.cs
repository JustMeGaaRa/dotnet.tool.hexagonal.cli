using CliFx;
using CliFx.Attributes;
using CliWrap;
using Microsoft.Extensions.Options;
using Silent.Tool.Hexagonal.Cli.Infrastructure.Options;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Silent.Tool.Hexagonal.Cli
{
    [Command("add webapp")]
    public class AddWebAppCommand : ICommand
    {
        private readonly IOptions<GeneralSection> _options;

        public AddWebAppCommand(IOptions<GeneralSection> options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        [CommandParameter(0)]
        public string WebAppName { get; set; }

        [CommandOption("--company", 'c')]
        public string Company { get; set; }

        [CommandOption("--framework", 'f')]
        public string Framework { get; set; }

        public async ValueTask ExecuteAsync(IConsole console)
        {
            await HandleProjectCreation(_options);
            await HandleProjectReferences(_options);
            await HandleSolutionReferences(_options);
        }

        private string GetServiceRelativePath(IOptions<GeneralSection> options, string serviceProjectType)
        {
            string serviceRelativePath = $"{options.Value.Projects.WebApp.Path}/{_options.Value.Projects.WebApp.Template}"
                .ReplaceToken(Constants.Tokens.CompanyTokenName, Company)
                .ReplaceToken(Constants.Tokens.ServiceTokenName, WebAppName)
                .ReplaceToken(Constants.Tokens.ProjectTypeTokenName, serviceProjectType);
            return serviceRelativePath;
        }

        private string GetServiceName(IOptions<GeneralSection> options, string serviceProjectType)
        {
            string serviceName = options.Value.Projects.Service.Template
                .ReplaceToken(Constants.Tokens.CompanyTokenName, Company)
                .ReplaceToken(Constants.Tokens.ServiceTokenName, WebAppName)
                .ReplaceToken(Constants.Tokens.ProjectTypeTokenName, serviceProjectType);
            return serviceName;
        }

        private async Task<bool> HandleProjectCreation(IOptions<GeneralSection> options)
        {
            var consoleOutputTarget = PipeTarget.ToStream(Console.OpenStandardOutput());
            string framework = Framework ?? options.Value.Framework.Default;

            string webappRelativePath = GetServiceRelativePath(options, Constants.ProjectTypes.Web);
            string webAppProjectName = GetServiceName(options, Constants.ProjectTypes.Web);

            // dotnet new webapp --name \"{WebAppProjectName}\" --output \"{webappRelativePath}\" --framework {framework}
            var webappCommand = CliWrap.Cli.Wrap("dotnet")
                .WithStandardOutputPipe(consoleOutputTarget)
                .WithArguments(args => args
                    .Add("new")
                    .Add("webapp")
                    .Add("--name")
                    .Add(webAppProjectName)
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

        private Task<bool> HandleProjectReferences(IOptions<GeneralSection> options)
        {
            return Task.FromResult(true);
        }

        private async Task<bool> HandleSolutionReferences(IOptions<GeneralSection> options)
        {
            if (Directory.GetFiles("./", "*.sln").Any())
            {
                var consoleOutputTarget = PipeTarget.ToStream(Console.OpenStandardOutput());

                string webappRelativePath = GetServiceRelativePath(options, Constants.ProjectTypes.Web);
                string webAppProjectName = GetServiceName(options, Constants.ProjectTypes.Web);

                var solutionServiceReferencesCommand = CliWrap.Cli.Wrap("dotnet")
                    .WithStandardOutputPipe(consoleOutputTarget)
                    .WithArguments(args => args
                        .Add("sln")
                        .Add("add")
                        .Add($"{webappRelativePath}/{webAppProjectName}.csproj")
                        .Add("--solution-folder")
                        .Add("src"));

                var solutionServiceReferencesResult = await solutionServiceReferencesCommand.ExecuteAsync();

                var results = new[]
                {
                    solutionServiceReferencesResult
                };

                return results.All(r => r.ExitCode == 0);

            }

            return false;
        }
    }
}
