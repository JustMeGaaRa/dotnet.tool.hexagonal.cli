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
    [Command("add service")]
    public class AddServiceCommand : ICommand
    {
        private readonly IOptions<GeneralSection> _options;

        public AddServiceCommand(IOptions<GeneralSection> options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        [CommandParameter(0)]
        public string ServiceName { get; set; }

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
            string serviceRelativePath = $"{options.Value.Projects.Service.Path}/{_options.Value.Projects.Service.Template}"
                .ReplaceToken(Constants.Tokens.CompanyTokenName, Company)
                .ReplaceToken(Constants.Tokens.ServiceTokenName, ServiceName)
                .ReplaceToken(Constants.Tokens.ProjectTypeTokenName, serviceProjectType);
            return serviceRelativePath;
        }

        private string GetServiceName(IOptions<GeneralSection> options, string serviceProjectType)
        {
            string serviceName = options.Value.Projects.Service.Template
                .ReplaceToken(Constants.Tokens.CompanyTokenName, Company)
                .ReplaceToken(Constants.Tokens.ServiceTokenName, ServiceName)
                .ReplaceToken(Constants.Tokens.ProjectTypeTokenName, serviceProjectType);
            return serviceName;
        }

        private string GetTestRelativePath(IOptions<GeneralSection> options, string testProjectType)
        {
            string testRelativePath = $"{options.Value.Projects.UnitTest.Path}/{_options.Value.Projects.UnitTest.Template}"
                .ReplaceToken(Constants.Tokens.CompanyTokenName, Company)
                .ReplaceToken(Constants.Tokens.ServiceTokenName, ServiceName)
                .ReplaceToken(Constants.Tokens.ProjectTypeTokenName, testProjectType);
            return testRelativePath;
        }

        private string GetTestName(IOptions<GeneralSection> options, string testProjectType)
        {
            string testName = options.Value.Projects.UnitTest.Template
                .ReplaceToken(Constants.Tokens.CompanyTokenName, Company)
                .ReplaceToken(Constants.Tokens.ServiceTokenName, ServiceName)
                .ReplaceToken(Constants.Tokens.ProjectTypeTokenName, testProjectType);
            return testName;
        }

        private async Task<bool> HandleProjectCreation(IOptions<GeneralSection> options)
        {
            var consoleOutputTarget = PipeTarget.ToStream(Console.OpenStandardOutput());
            string framework = Framework ?? options.Value.Framework.Default;

            string domainRelativePath = GetServiceRelativePath(_options, Constants.ProjectTypes.Domain);
            string domainProjectName = GetServiceName(_options, Constants.ProjectTypes.Domain);

            string infraRelativePath = GetServiceRelativePath(_options, Constants.ProjectTypes.Infrastructure);
            string infraProjectName = GetServiceName(_options, Constants.ProjectTypes.Infrastructure);

            string webapiRelativePath = GetServiceRelativePath(_options, Constants.ProjectTypes.Api);
            string webapiProjectName = GetServiceName(_options, Constants.ProjectTypes.Api);

            string testRelativePath = GetTestRelativePath(_options, Constants.ProjectTypes.UnitTest);
            string testProjectName = GetTestName(_options, Constants.ProjectTypes.UnitTest);

            // dotnet new classlib --name \"{domainProjectName}\" --output \"{domainRelativePath}\" --framework {framework}
            var domainCommand = CliWrap.Cli.Wrap("dotnet")
                .WithStandardOutputPipe(consoleOutputTarget)
                .WithArguments(args => args
                    .Add("new")
                    .Add("classlib")
                    .Add("--name")
                    .Add(domainProjectName)
                    .Add("--output")
                    .Add(domainRelativePath)
                    .Add("--framework")
                    .Add(framework));

            // dotnet new classlib --name \"{infraProjectName}\" --output \"{infraRelativePath}\" --framework {framework}
            var infraCommand = CliWrap.Cli.Wrap("dotnet")
                .WithStandardOutputPipe(consoleOutputTarget)
                .WithArguments(args => args
                    .Add("new")
                    .Add("classlib")
                    .Add("--name")
                    .Add(infraProjectName)
                    .Add("--output")
                    .Add(infraRelativePath)
                    .Add("--framework")
                    .Add(framework));

            // dotnet new webapi --name \"{webapiProjectName}\" --output \"{webapiRelativePath}\" --framework {framework}
            var webapiCommand = CliWrap.Cli.Wrap("dotnet")
                .WithStandardOutputPipe(consoleOutputTarget)
                .WithArguments(args => args
                    .Add("new")
                    .Add("webapi")
                    .Add("--name")
                    .Add(webapiProjectName)
                    .Add("--output")
                    .Add(webapiRelativePath)
                    .Add("--framework")
                    .Add(framework));

            // dotnet new xunit --name \"{testProjectName}\" --output \"{testRelativePath}\" --framework {framework}
            var unitTestCommand = CliWrap.Cli.Wrap("dotnet")
                .WithStandardOutputPipe(consoleOutputTarget)
                .WithArguments(args => args
                    .Add("new")
                    .Add("xunit")
                    .Add("--name")
                    .Add(testProjectName)
                    .Add("--output")
                    .Add(testRelativePath)
                    .Add("--framework")
                    .Add(framework));

            var domainResult = await domainCommand.ExecuteAsync();
            var infraResult = await infraCommand.ExecuteAsync();
            var webapiResult = await webapiCommand.ExecuteAsync();
            var unitTestResult = await unitTestCommand.ExecuteAsync();

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

            string domainRelativePath = GetServiceRelativePath(_options, Constants.ProjectTypes.Domain);
            string infraRelativePath = GetServiceRelativePath(_options, Constants.ProjectTypes.Infrastructure);
            string webapiRelativePath = GetServiceRelativePath(_options, Constants.ProjectTypes.Api);
            string testRelativePath = GetTestRelativePath(_options, Constants.ProjectTypes.UnitTest);

            // dotnet add {infraRelativePath} reference {domainRelativePath}
            var infraReferencesCommand = CliWrap.Cli.Wrap("dotnet")
                .WithStandardOutputPipe(consoleOutputTarget)
                .WithArguments(args => args
                    .Add("add")
                    .Add(infraRelativePath)
                    .Add("reference")
                    .Add(domainRelativePath));

            // dotnet add {webapiRelativePath} reference {domainRelativePath} {infraRelativePath}
            var apiReferencesCommand = CliWrap.Cli.Wrap("dotnet")
                .WithStandardOutputPipe(consoleOutputTarget)
                .WithArguments(args => args
                    .Add("add")
                    .Add(webapiRelativePath)
                    .Add("reference")
                    .Add(domainRelativePath)
                    .Add(infraRelativePath));

            // dotnet add {testRelativePath} reference {domainRelativePath}
            var testReferencesCommand = CliWrap.Cli.Wrap("dotnet")
                .WithStandardOutputPipe(consoleOutputTarget)
                .WithArguments(args => args
                    .Add("add")
                    .Add(testRelativePath)
                    .Add("reference")
                    .Add(domainRelativePath));

            var infraReferencesResult = await infraReferencesCommand.ExecuteAsync();
            var apiReferencesResult = await apiReferencesCommand.ExecuteAsync();
            var testReferencesResult = await testReferencesCommand.ExecuteAsync();

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

                string domainRelativePath = GetServiceRelativePath(_options, Constants.ProjectTypes.Domain);
                string domainProjectName = GetServiceName(_options, Constants.ProjectTypes.Domain);

                string infraRelativePath = GetServiceRelativePath(_options, Constants.ProjectTypes.Infrastructure);
                string infraProjectName = GetServiceName(_options, Constants.ProjectTypes.Infrastructure);

                string webapiRelativePath = GetServiceRelativePath(_options, Constants.ProjectTypes.Api);
                string webapiProjectName = GetServiceName(_options, Constants.ProjectTypes.Api);

                string testRelativePath = GetTestRelativePath(_options, Constants.ProjectTypes.UnitTest);
                string testProjectName = GetTestName(_options, Constants.ProjectTypes.UnitTest);

                var solutionServiceReferencesCommand = CliWrap.Cli.Wrap("dotnet")
                    .WithStandardOutputPipe(consoleOutputTarget)
                    .WithArguments(args => args
                        .Add("sln")
                        .Add("add")
                        .Add($"{domainRelativePath}/{domainProjectName}.csproj")
                        .Add($"{infraRelativePath}/{infraProjectName}.csproj")
                        .Add($"{webapiRelativePath}/{webapiProjectName}.csproj")
                        .Add("--solution-folder")
                        .Add($"src\\{options.Value.Folders.ServicesFolder}\\{ServiceName}"));

                var solutionTestReferencesCommand = CliWrap.Cli.Wrap("dotnet")
                    .WithStandardOutputPipe(consoleOutputTarget)
                    .WithArguments(args => args
                        .Add("sln")
                        .Add("add")
                        .Add($"{testRelativePath}/{testProjectName}.csproj")
                        .Add("--solution-folder")
                        .Add("test"));

                var solutionServiceReferencesResult = await solutionServiceReferencesCommand.ExecuteAsync();
                var solutionTestReferencesResult = await solutionTestReferencesCommand.ExecuteAsync();

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
