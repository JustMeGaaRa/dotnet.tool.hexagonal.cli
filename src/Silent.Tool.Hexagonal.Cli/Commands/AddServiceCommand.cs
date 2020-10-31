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

        public string DomainProjectName => $"{ServiceName}.Domain";

        public string InfraProjectName => $"{ServiceName}.Infrastructure";

        public string WebapiProjectName => $"{ServiceName}.Api";

        public string TestProjectName => $"{ServiceName}.Domain.Tests";

        public async ValueTask ExecuteAsync(IConsole console)
        {
            await HandleProjectCreation(_options);
            await HandleProjectReferences(_options);
            await HandleSolutionReferences(_options);
        }

        private string GetServiceRelativePath(IOptions<GeneralOptions> options, string serviceProjectName)
        {
            string serviceRelativePath = $"src/{options.Value.Folders.ServicesFolder}/{ServiceName}/{serviceProjectName}";
            return serviceRelativePath;
        }

        private string GetTestRelativePath(IOptions<GeneralOptions> options, string testProjectName)
        {
            string testRelativePath = $"test/{testProjectName}";
            return testRelativePath;
        }

        private async Task<bool> HandleProjectCreation(IOptions<GeneralOptions> options)
        {
            var consoleOutputTarget = PipeTarget.ToStream(Console.OpenStandardOutput());
            string framework = Framework ?? options.Value.Framework.Default;

            string domainRelativePath = GetServiceRelativePath(options, DomainProjectName);
            string infraRelativePath = GetServiceRelativePath(options, InfraProjectName);
            string webapiRelativePath = GetServiceRelativePath(options, WebapiProjectName);
            string testRelativePath = GetTestRelativePath(options, TestProjectName);

            // dotnet new classlib --name \"{domainProjectName}\" --output \"{domainRelativePath}\" --framework {framework}
            var domainCommand = CliWrap.Cli.Wrap("dotnet")
                .WithStandardOutputPipe(consoleOutputTarget)
                .WithArguments(args => args
                    .Add("new")
                    .Add("classlib")
                    .Add("--name")
                    .Add(DomainProjectName)
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
                    .Add(InfraProjectName)
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
                    .Add(WebapiProjectName)
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
                    .Add(TestProjectName)
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

        private async Task<bool> HandleProjectReferences(IOptions<GeneralOptions> options)
        {
            var consoleOutputTarget = PipeTarget.ToStream(Console.OpenStandardOutput());
            string domainRelativePath = GetServiceRelativePath(options, DomainProjectName);
            string infraRelativePath = GetServiceRelativePath(options, InfraProjectName);
            string webapiRelativePath = GetServiceRelativePath(options, WebapiProjectName);
            string testRelativePath = GetTestRelativePath(options, TestProjectName);

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

        private async Task<bool> HandleSolutionReferences(IOptions<GeneralOptions> options)
        {
            var consoleOutputTarget = PipeTarget.ToStream(Console.OpenStandardOutput());
            string domainRelativePath = GetServiceRelativePath(options, DomainProjectName);
            string infraRelativePath = GetServiceRelativePath(options, InfraProjectName);
            string webapiRelativePath = GetServiceRelativePath(options, WebapiProjectName);
            string testRelativePath = GetTestRelativePath(options, TestProjectName);

            var solutionServiceReferencesCommand = CliWrap.Cli.Wrap("dotnet")
                .WithStandardOutputPipe(consoleOutputTarget)
                .WithArguments(args => args
                    .Add("sln")
                    .Add("add")
                    .Add($"{domainRelativePath}/{DomainProjectName}.csproj")
                    .Add($"{infraRelativePath}/{InfraProjectName}.csproj")
                    .Add($"{webapiRelativePath}/{WebapiProjectName}.csproj")
                    .Add("--solution-folder")
                    .Add($"src\\{options.Value.Folders.ServicesFolder}\\{ServiceName}"));

            var solutionTestReferencesCommand = CliWrap.Cli.Wrap("dotnet")
                .WithStandardOutputPipe(consoleOutputTarget)
                .WithArguments(args => args
                    .Add("sln")
                    .Add("add")
                    .Add($"{testRelativePath}/{TestProjectName}.csproj")
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
    }
}
