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

        private string GetServiceRelativePath(IOptions<GeneralSection> options)
        {
            return $"{options.Value.Projects.Service.Path}\\{options.Value.Projects.Service.Template}";
        }

        private string GetTestRelativePath(IOptions<GeneralSection> options)
        {
            return $"{options.Value.Projects.UnitTest.Path}\\{options.Value.Projects.UnitTest.Template}";
        }

        private async Task<bool> HandleProjectCreation(IOptions<GeneralSection> options)
        {
            var consoleOutputTarget = PipeTarget.ToStream(Console.OpenStandardOutput());
            string framework = Framework ?? options.Value.Framework.Default;

            var domainRelativePath = ResolveAllTokens(GetServiceRelativePath(options), ProjectTypes.Domain);
            var domainProjectName = ResolveAllTokens(options.Value.Projects.Service.Template, ProjectTypes.Domain);

            var infraRelativePath = ResolveAllTokens(GetServiceRelativePath(options), ProjectTypes.Infrastructure);
            var infraProjectName = ResolveAllTokens(options.Value.Projects.Service.Template, ProjectTypes.Infrastructure);

            var webapiRelativePath = ResolveAllTokens(GetServiceRelativePath(options), ProjectTypes.Api);
            var webapiProjectName = ResolveAllTokens(options.Value.Projects.Service.Template, ProjectTypes.Api);

            var testRelativePath = ResolveAllTokens(GetTestRelativePath(options), ProjectTypes.UnitTest);
            var testProjectName = ResolveAllTokens(options.Value.Projects.UnitTest.Template, ProjectTypes.UnitTest);

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

            var domainResult = options.Value.Projects.Service.Generate
                ? await domainCommand.ExecuteAsync()
                : _defaultCommandResult;
            var infraResult = options.Value.Projects.Service.Generate
                ? await infraCommand.ExecuteAsync()
                : _defaultCommandResult;
            var webapiResult = options.Value.Projects.Service.Generate
                ? await webapiCommand.ExecuteAsync()
                : _defaultCommandResult;
            var unitTestResult = options.Value.Projects.UnitTest.Generate
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

            var domainRelativePath = ResolveAllTokens(GetServiceRelativePath(options), ProjectTypes.Domain);
            var infraRelativePath = ResolveAllTokens(GetServiceRelativePath(options), ProjectTypes.Infrastructure);
            var webapiRelativePath = ResolveAllTokens(GetServiceRelativePath(options), ProjectTypes.Api);
            var testRelativePath = ResolveAllTokens(GetTestRelativePath(options), ProjectTypes.UnitTest);

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
                    .Add(testRelativePath.Value)
                    .Add("reference")
                    .Add(domainRelativePath.Value));

            var infraReferencesResult = options.Value.Projects.Service.Generate
                ? await infraReferencesCommand.ExecuteAsync()
                : _defaultCommandResult;
            var apiReferencesResult = options.Value.Projects.Service.Generate
                ? await apiReferencesCommand.ExecuteAsync()
                : _defaultCommandResult;
            var testReferencesResult = options.Value.Projects.Service.Generate
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

                var serviceOutputPath = ResolveAllTokens(options.Value.Projects.Service.Path, string.Empty);
                var testOutputPath = ResolveAllTokens(options.Value.Projects.UnitTest.Path, string.Empty);

                var domainRelativePath = ResolveAllTokens(GetServiceRelativePath(options), ProjectTypes.Domain);
                var domainProjectName = ResolveAllTokens(options.Value.Projects.Service.Template, ProjectTypes.Domain);

                var infraRelativePath = ResolveAllTokens(GetServiceRelativePath(options), ProjectTypes.Infrastructure);
                var infraProjectName = ResolveAllTokens(options.Value.Projects.Service.Template, ProjectTypes.Infrastructure);

                var webapiRelativePath = ResolveAllTokens(GetServiceRelativePath(options), ProjectTypes.Api);
                var webapiProjectName = ResolveAllTokens(options.Value.Projects.Service.Template, ProjectTypes.Api);

                var testRelativePath = ResolveAllTokens(GetTestRelativePath(options), ProjectTypes.UnitTest);
                var testProjectName = ResolveAllTokens(options.Value.Projects.UnitTest.Template, ProjectTypes.UnitTest);

                var solutionServiceReferencesCommand = CliWrap.Cli.Wrap("dotnet")
                    .WithStandardOutputPipe(consoleOutputTarget)
                    .WithArguments(args => args
                        .Add("sln")
                        .Add("add")
                        .Add($"{domainRelativePath.Value}\\{domainProjectName.Value}.csproj")
                        .Add($"{infraRelativePath.Value}\\{infraProjectName.Value}.csproj")
                        .Add($"{webapiRelativePath.Value}\\{webapiProjectName.Value}.csproj")
                        .Add("--solution-folder")
                        .Add(serviceOutputPath.Value));

                var solutionTestReferencesCommand = CliWrap.Cli.Wrap("dotnet")
                    .WithStandardOutputPipe(consoleOutputTarget)
                    .WithArguments(args => args
                        .Add("sln")
                        .Add("add")
                        .Add($"{testRelativePath.Value}\\{testProjectName.Value}.csproj")
                        .Add("--solution-folder")
                        .Add(testOutputPath.Value));

                var solutionServiceReferencesResult = options.Value.Projects.Service.Generate
                    ? await solutionServiceReferencesCommand.ExecuteAsync()
                    : _defaultCommandResult;
                var solutionTestReferencesResult = options.Value.Projects.UnitTest.Generate
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
