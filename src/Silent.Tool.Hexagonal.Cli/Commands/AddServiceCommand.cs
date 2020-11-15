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
    [Command("add service")]
    public class AddServiceCommand : ICommand
    {
        private readonly IOptions<GeneralSection> _options;
        private readonly CommandResult _defaultCommandResult;

        public AddServiceCommand(IOptions<GeneralSection> options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _defaultCommandResult = new CommandResult(0, DateTime.UtcNow, DateTime.UtcNow);
        }

        [CommandParameter(0)]
        public string ServiceName { get; set; }

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
                .ResolveToken(TokenTypes.Service, ServiceName)
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
            var framework = Framework ?? options.Value.Framework.Default;
            var projectOptions = options.Value.Projects;

            var serviceOutputPath = GetOutputPath(projectOptions.Service.Path, projectOptions.Service.Template);
            var domainRelativePath = ResolveAllTokens(serviceOutputPath, ProjectTypes.Domain);
            var domainProjectName = ResolveAllTokens(projectOptions.Service.Template, ProjectTypes.Domain);
            var infraRelativePath = ResolveAllTokens(serviceOutputPath, ProjectTypes.Infrastructure);
            var infraProjectName = ResolveAllTokens(projectOptions.Service.Template, ProjectTypes.Infrastructure);
            var webapiRelativePath = ResolveAllTokens(serviceOutputPath, ProjectTypes.Api);
            var webapiProjectName = ResolveAllTokens(projectOptions.Service.Template, ProjectTypes.Api);

            var uniTestOutputPath = GetOutputPath(projectOptions.UnitTest.Path, projectOptions.UnitTest.Template);
            var testProjectName = ResolveAllTokens(projectOptions.UnitTest.Template, ProjectTypes.UnitTest);
            var testRelativePath = ResolveAllTokens(uniTestOutputPath, ProjectTypes.UnitTest);

            // dotnet new classlib --name {domainProjectName} --output {domainRelativePath} --framework {framework}
            var domainCommand = CliWrap.Cli.Wrap("dotnet")
                .WithStandardOutputPipe(consoleOutputTarget)
                .WithArguments(args => args
                    .Add("new")
                    .Add("classlib")
                    .Add("--name")
                    .Add(domainProjectName.Value)
                    .Add("--output")
                    .Add(domainRelativePath.Value)
                    .Add("--framework")
                    .Add(framework));

            // dotnet new classlib --name {infraProjectName} --output {infraRelativePath} --framework {framework}
            var infraCommand = CliWrap.Cli.Wrap("dotnet")
                .WithStandardOutputPipe(consoleOutputTarget)
                .WithArguments(args => args
                    .Add("new")
                    .Add("classlib")
                    .Add("--name")
                    .Add(infraProjectName.Value)
                    .Add("--output")
                    .Add(infraRelativePath.Value)
                    .Add("--framework")
                    .Add(framework));

            // dotnet new webapi --name {webapiProjectName} --output {webapiRelativePath} --framework {framework}
            var webapiCommand = CliWrap.Cli.Wrap("dotnet")
                .WithStandardOutputPipe(consoleOutputTarget)
                .WithArguments(args => args
                    .Add("new")
                    .Add("webapi")
                    .Add("--name")
                    .Add(webapiProjectName.Value)
                    .Add("--output")
                    .Add(webapiRelativePath.Value)
                    .Add("--framework")
                    .Add(framework));

            // dotnet new xunit --name {testProjectName} --output {testRelativePath} --framework {framework}
            var unitTestCommand = CliWrap.Cli.Wrap("dotnet")
                .WithStandardOutputPipe(consoleOutputTarget)
                .WithArguments(args => args
                    .Add("new")
                    .Add("xunit")
                    .Add("--name")
                    .Add(testProjectName.Value)
                    .Add("--output")
                    .Add(testRelativePath.Value)
                    .Add("--framework")
                    .Add(framework));

            PrintUnresolvedTokensError(domainRelativePath);
            PrintUnresolvedTokensError(infraRelativePath);
            PrintUnresolvedTokensError(webapiRelativePath);
            PrintUnresolvedTokensError(testRelativePath);

            var domainResult = projectOptions.Service.Generate
                && domainRelativePath.IsResolved
                && domainProjectName.IsResolved
                ? await domainCommand.ExecuteAsync()
                : _defaultCommandResult;
            var infraResult = projectOptions.Service.Generate
                && infraRelativePath.IsResolved
                && infraProjectName.IsResolved
                ? await infraCommand.ExecuteAsync()
                : _defaultCommandResult;
            var webapiResult = projectOptions.Service.Generate
                && webapiRelativePath.IsResolved
                && webapiProjectName.IsResolved
                ? await webapiCommand.ExecuteAsync()
                : _defaultCommandResult;
            var unitTestResult = projectOptions.UnitTest.Generate
                && testRelativePath.IsResolved
                && testProjectName.IsResolved
                ? await unitTestCommand.ExecuteAsync()
                : _defaultCommandResult;

            var results = new[]
            {
                domainResult,
                infraResult,
                webapiResult,
                unitTestResult
            };

            return results.All(r => r.ExitCode == 0);
        }

        private async Task<bool> HandleProjectReferences(IOptions<GeneralSection> options)
        {
            var consoleOutputTarget = PipeTarget.ToStream(Console.OpenStandardOutput());
            var projectOptions = options.Value.Projects;

            var serviceOutputPath = GetOutputPath(projectOptions.Service.Path, projectOptions.Service.Template);
            var domainRelativePath = ResolveAllTokens(serviceOutputPath, ProjectTypes.Domain);
            var infraRelativePath = ResolveAllTokens(serviceOutputPath, ProjectTypes.Infrastructure);
            var webapiRelativePath = ResolveAllTokens(serviceOutputPath, ProjectTypes.Api);

            var uniTestOutputPath = GetOutputPath(projectOptions.UnitTest.Path, projectOptions.UnitTest.Template);
            var unitTestRelativePath = ResolveAllTokens(uniTestOutputPath, ProjectTypes.UnitTest);

            // dotnet add {infraRelativePath} reference {domainRelativePath}
            var infraReferencesCommand = CliWrap.Cli.Wrap("dotnet")
                .WithStandardOutputPipe(consoleOutputTarget)
                .WithArguments(args => args
                    .Add("add")
                    .Add(infraRelativePath.Value)
                    .Add("reference")
                    .Add(domainRelativePath.Value));

            // dotnet add {webapiRelativePath} reference {domainRelativePath} {infraRelativePath}
            var apiReferencesCommand = CliWrap.Cli.Wrap("dotnet")
                .WithStandardOutputPipe(consoleOutputTarget)
                .WithArguments(args => args
                    .Add("add")
                    .Add(webapiRelativePath.Value)
                    .Add("reference")
                    .Add(domainRelativePath.Value)
                    .Add(infraRelativePath.Value));

            // dotnet add {testRelativePath} reference {domainRelativePath}
            var testReferencesCommand = CliWrap.Cli.Wrap("dotnet")
                .WithStandardOutputPipe(consoleOutputTarget)
                .WithArguments(args => args
                    .Add("add")
                    .Add(unitTestRelativePath.Value)
                    .Add("reference")
                    .Add(domainRelativePath.Value));

            var infraReferencesResult = projectOptions.Service.Generate
                ? await infraReferencesCommand.ExecuteAsync()
                : _defaultCommandResult;
            var apiReferencesResult = projectOptions.Service.Generate
                ? await apiReferencesCommand.ExecuteAsync()
                : _defaultCommandResult;
            var testReferencesResult = projectOptions.Service.Generate
                ? await testReferencesCommand.ExecuteAsync()
                : _defaultCommandResult;

            var results = new[]
            {
                infraReferencesResult,
                apiReferencesResult,
                testReferencesResult
            };

            return results.All(r => r.ExitCode == 0);
        }

        private async Task<bool> HandleSolutionReferences(IOptions<GeneralSection> options)
        {
            if (Directory.GetFiles("./", "*.sln").Any())
            {
                var consoleOutputTarget = PipeTarget.ToStream(Console.OpenStandardOutput());
                var projectOptions = options.Value.Projects;

                var serviceOutputPath = ResolveAllTokens(projectOptions.Service.Path, ProjectTypes.Service);
                var serviceProjectPath = GetProjectPath(projectOptions.Service.Path, projectOptions.Service.Template);

                var domainRelativePath = ResolveAllTokens(serviceProjectPath, ProjectTypes.Domain);
                var infraRelativePath = ResolveAllTokens(serviceProjectPath, ProjectTypes.Infrastructure);
                var webapiRelativePath = ResolveAllTokens(serviceProjectPath, ProjectTypes.Api);

                var testOutputPath = ResolveAllTokens(projectOptions.UnitTest.Path, ProjectTypes.UnitTest);
                var testProjectPath = GetProjectPath(projectOptions.UnitTest.Path, projectOptions.UnitTest.Template);
                var testRelativePath = ResolveAllTokens(testProjectPath, ProjectTypes.UnitTest);

                var solutionServiceReferencesCommand = CliWrap.Cli.Wrap("dotnet")
                    .WithStandardOutputPipe(consoleOutputTarget)
                    .WithArguments(args => args
                        .Add("sln")
                        .Add("add")
                        .Add(domainRelativePath.Value)
                        .Add(infraRelativePath.Value)
                        .Add(webapiRelativePath.Value)
                        .Add("--solution-folder")
                        .Add(serviceOutputPath.Value));

                var solutionTestReferencesCommand = CliWrap.Cli.Wrap("dotnet")
                    .WithStandardOutputPipe(consoleOutputTarget)
                    .WithArguments(args => args
                        .Add("sln")
                        .Add("add")
                        .Add(testRelativePath.Value)
                        .Add("--solution-folder")
                        .Add(testOutputPath.Value));

                var solutionServiceReferencesResult = projectOptions.Service.Generate
                    && domainRelativePath.IsResolved
                    && infraRelativePath.IsResolved
                    && webapiRelativePath.IsResolved
                    && testRelativePath.IsResolved
                    && serviceOutputPath.IsResolved
                    ? await solutionServiceReferencesCommand.ExecuteAsync()
                    : _defaultCommandResult;
                var solutionTestReferencesResult = projectOptions.UnitTest.Generate
                    && testRelativePath.IsResolved
                    && testOutputPath.IsResolved
                    ? await solutionTestReferencesCommand.ExecuteAsync()
                    : _defaultCommandResult;

                var results = new[]
                {
                    solutionServiceReferencesResult,
                    solutionTestReferencesResult
                };

                return results.All(x => x.ExitCode == 0);
            }

            return false;
        }
    }
}
