using CliFx;
using CliFx.Attributes;
using CliWrap;
using Microsoft.Extensions.Options;
using Silent.Tool.Hexagonal.Cli.Infrastructure.Options;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Threading.Tasks;

namespace Silent.Tool.Hexagonal.Cli
{
    [Command("init")]
    public class InitializeCommand : ICommand
    {
        private readonly IOptions<GeneralSection> _options;

        public InitializeCommand(IOptions<GeneralSection> options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        [CommandParameter(0)]
        public string ProjectName { get; set; }

        public async ValueTask ExecuteAsync(IConsole console)
        {
            await HandleIniCommand(_options);
        }

        private async Task<bool> HandleIniCommand(IOptions<GeneralSection> options)
        {
            var consoleOutputTarget = PipeTarget.ToStream(Console.OpenStandardOutput());

            // dotnet new sln --name {ProjectName} --output {ProjectName}
            var domainCommand = CliWrap.Cli.Wrap("dotnet")
                .WithStandardOutputPipe(consoleOutputTarget)
                .WithArguments(args => args
                    .Add("new")
                    .Add("sln")
                    .Add("--name")
                    .Add(ProjectName));

            var domainCommandResult = await domainCommand.ExecuteAsync();

            return CopyDefaultConfig();
        }

        private static bool CopyDefaultConfig()
        {
            try
            {
                var executingAssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var defaultConfigPath = Path.Combine(executingAssemblyPath, Constants.GlobalFileName);
                var defaultConfigJson = File.ReadAllText(defaultConfigPath);
                var currentDirectoryPath = Directory.GetCurrentDirectory();
                var currentDirectoryConfigPath = Path.Combine(currentDirectoryPath, Constants.ConfigFileName);
                File.WriteAllText(currentDirectoryConfigPath, defaultConfigJson);
                Console.WriteLine($"The default config with name '{Constants.ConfigFileName}' was copied to current directory.");
                return true;
            }
            catch (PathTooLongException ex)
            {
                Console.WriteLine($"The path to the file is too long. Could not save the settings. Error: {ex}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"The access to the file is unauthorized to this app. Error: {ex}");
            }
            catch (SecurityException ex)
            {
                Console.WriteLine($"The security issue occured when trying to access the file. Error: {ex}");
            }

            return false;
        }
    }
}
