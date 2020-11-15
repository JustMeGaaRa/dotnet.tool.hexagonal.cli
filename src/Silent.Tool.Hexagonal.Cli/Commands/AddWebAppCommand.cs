using CliFx;
using CliFx.Attributes;
using CliWrap;
using Microsoft.Extensions.Options;
using Silent.Tool.Hexagonal.Cli.Infrastructure.Options;
using Silent.Tool.Hexagonal.Cli.Models;
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
        private readonly CommandResult _defaultCommandResult;

        public AddWebAppCommand(IOptions<GeneralSection> options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _defaultCommandResult = new CommandResult(0, DateTime.UtcNow, DateTime.UtcNow);
        }

        [CommandParameter(0)]
        public string WebAppName { get; set; }

        [CommandOption("--company", 'c')]
        public string CompanyName { get; set; }

        [CommandOption("--framework", 'f')]
        public string Framework { get; set; }

        public async ValueTask ExecuteAsync(IConsole console)
        {
            await HandleProjectCreation(_options);
            await HandleProjectReferences(_options);
            await HandleSolutionReferences(_options);
        }

        private Template ResolveAllTokens(string format, string projectType)
        {
            var resolvedTemplate = Template
                .FromFormat(format)
                .ResolveToken(TokenTypes.Company, CompanyName)
                .ResolveToken(TokenTypes.WebApp, WebAppName)
                .ResolveToken(TokenTypes.ProjectType, projectType);
            return resolvedTemplate;
        }

        private string GetOutputPath(string relativePath, string projectName)
        {
            return $"{relativePath}\\{projectName}";
        }

        private string GetProjectPath(string relativePath, string projectName)
        {
            return $"{relativePath}\\{projectName}\\{projectName}.csproj";
        }

        private void PrintUnresolvedTokensError(Template template)
        {
            if (!template.IsResolved)
            {
                Console.WriteLine($"The following template was not fully resolved: '{template.Value}'. Please, resolve the following tokens:");
                template.UnresolvedTokens.ToList().ForEach(x => Console.WriteLine($"\t- {x.Name}"));
            }
        }

        private async Task<bool> HandleProjectCreation(IOptions<GeneralSection> options)
        {
            var consoleOutputTarget = PipeTarget.ToStream(Console.OpenStandardOutput());
            string framework = Framework ?? options.Value.Framework.Default;
            var projectOptions = options.Value.Projects;

            var webAppOutputPath = GetOutputPath(projectOptions.WebApp.Path, projectOptions.WebApp.Template);
            var webAppRelativePath = ResolveAllTokens(webAppOutputPath, ProjectTypes.Web);
            var webAppProjectName = ResolveAllTokens(projectOptions.WebApp.Template, ProjectTypes.Web);

            // dotnet new webapp --name {WebAppProjectName} --output {webappRelativePath} --framework {framework}
            var webappCommand = CliWrap.Cli.Wrap("dotnet")
                .WithStandardOutputPipe(consoleOutputTarget)
                .WithArguments(args => args
                    .Add("new")
                    .Add("webapp")
                    .Add("--name")
                    .Add(webAppProjectName.Value)
                    .Add("--output")
                    .Add(webAppRelativePath.Value)
                    .Add("--framework")
                    .Add(framework));

            PrintUnresolvedTokensError(webAppRelativePath);

            var webappResult = projectOptions.WebApp.Generate
                && webAppProjectName.IsResolved
                && webAppRelativePath.IsResolved
                ? await webappCommand.ExecuteAsync()
                : _defaultCommandResult;

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
                var projectOptions = options.Value.Projects;

                var webAppOutputPath = ResolveAllTokens(projectOptions.WebApp.Path, ProjectTypes.Web);
                var webAppProjectPath = GetProjectPath(projectOptions.WebApp.Path, projectOptions.WebApp.Template);
                var webAppRelativePath = ResolveAllTokens(webAppProjectPath, ProjectTypes.Web);

                var solutionServiceReferencesCommand = CliWrap.Cli.Wrap("dotnet")
                    .WithStandardOutputPipe(consoleOutputTarget)
                    .WithArguments(args => args
                        .Add("sln")
                        .Add("add")
                        .Add(webAppRelativePath.Value)
                        .Add("--solution-folder")
                        .Add(webAppOutputPath.Value));

                var solutionServiceReferencesResult = options.Value.Projects.WebApp.Generate
                    ? await solutionServiceReferencesCommand.ExecuteAsync()
                    : _defaultCommandResult;

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
