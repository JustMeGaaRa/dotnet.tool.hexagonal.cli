using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Silent.Tool.Hexagonal.Cli
{
    internal class Program
    {
        private const string framework = "framework";
        private const string clients = "clients_folder";
        private const string docs = "docs_folder";
        private const string samples = "samples_folder";
        private const string services = "services_folder";
        private const string webapps = "webapps_folder";
        private static Dictionary<string, string> options = new Dictionary<string, string>
            {
                { framework, "netcoreapp3.1" },
                { clients, "clients" },
                { docs, "docs" },
                { samples, "samples" },
                { services, "services" },
                { webapps, "webapps" }
            };

        private static void Main(string[] args)
        {
            // dotnet hexa
            // dotnet hexa help
            // dotnet hexa init "Shop" --framework "net5.0"
            // dotnet hexa add service "Orders" --framework "net5.0"
            // dotnet hexa add service --help
            // dotnet hexa add webapp "Marketplace" --framework "net5.0"
            // dotnet hexa add webapp --help
            // dotnet hexa add component "COMPONENT_NAME" --service "SERVICE_NAME"

            HandleBaseCommand(args);
        }

        private static bool HandleBaseCommand(string[] args)
        {
            return args?[0] switch
            {
                "init"  => HandleIniCommand(args[1..]),
                "add"   => HandleAddCommand(args[1..]),
                "help"  => HandleHelpCommand(),
                _       => HandleHelpCommand()
            };
        }

        private static bool HandleHelpCommand()
        {
            throw new NotImplementedException();
        }

        private static bool HandleIniCommand(string[] args)
        {
            string projectName = GetArgumentValue(args);
            string solutionCommand = $"dotnet new sln --name \"{projectName}\" --output \"{projectName}\"";

            var successfull = true;
            successfull &= CreateDirectorySafely($"{projectName}/{options[clients]}");
            successfull &= CreateDirectorySafely($"{projectName}/{options[docs]}");
            successfull &= CreateDirectorySafely($"{projectName}/{options[samples]}");
            successfull &= CreateDirectorySafely($"{projectName}/src/{options[services]}");
            successfull &= CreateDirectorySafely($"{projectName}/src/{options[webapps]}");
            successfull &= CreateDirectorySafely($"{projectName}/test");
            successfull &= RunCommandPromptCommand(solutionCommand);
            
            return successfull;
        }

        private static bool HandleAddCommand(string[] args)
        {
            return args[0] switch
            {
                "service"   => HandleAddServiceCommand(args[1..]),
                "webapp"    => HandleAddWebAppCommand(args[1..]),
                "component" => HandleAddComponentCommand(args[1..]),
                _           => false
            };
        }

        private static bool HandleAddServiceCommand(string[] args)
        {
            string serviceName = GetArgumentValue(args);
            string frameworkOption = GetOptionsValue(args, "--framework") ?? options[framework];
            string serviceRelativePath = $"src/{options[services]}/{serviceName}";

            string domainProjectName = $"{serviceName}.Domain";
            string infraProjectName = $"{serviceName}.Infrastructure";
            string webapiProjectName = $"{serviceName}.Api";
            string testProjectName = $"{serviceName}.Domain.Tests";

            string domainRelativePath = $"{serviceRelativePath}/{domainProjectName}";
            string infraRelativePath = $"{serviceRelativePath}/{infraProjectName}";
            string webapiRelativePath = $"{serviceRelativePath}/{webapiProjectName}";
            string testRelativePath = $"test/{testProjectName}";

            string domainCommand = $"dotnet new classlib --name \"{domainProjectName}\" --output \"{domainRelativePath}\" --framework {frameworkOption}";
            string infraCommand = $"dotnet new classlib --name \"{infraProjectName}\" --output \"{infraRelativePath}\" --framework {frameworkOption}";
            string webapiCommand = $"dotnet new webapi --name \"{webapiProjectName}\" --output \"{webapiRelativePath}\" --framework {frameworkOption}";
            string unitTestCommand = $"dotnet new xunit --name \"{testProjectName}\" --output \"{testRelativePath}\" --framework {frameworkOption}";

            var successfull = true;
            successfull = successfull && RunCommandPromptCommand(domainCommand);
            successfull = successfull && RunCommandPromptCommand(infraCommand);
            successfull = successfull && RunCommandPromptCommand(webapiCommand);
            successfull = successfull && RunCommandPromptCommand(unitTestCommand);

            return successfull;
        }

        private static bool HandleAddWebAppCommand(string[] args)
        {
            string serviceName = GetArgumentValue(args);
            string frameworkOption = GetOptionsValue(args, "--framework") ?? options[framework];
            string serviceRelativePath = $"src/{options[services]}/{serviceName}";
            string webappProjectName = $"{serviceName}.Web";
            string webappRelativePath = $"{serviceRelativePath}/{webappProjectName}";
            string webappCommand = $"dotnet new webapi --name \"{webappProjectName}\" --output \"{webappRelativePath}\" --framework {frameworkOption}";

            var successfull = true;
            successfull = successfull && RunCommandPromptCommand(webappCommand);

            return successfull;
        }

        private static bool HandleAddComponentCommand(string[] args)
        {
            throw new NotImplementedException();
        }

        private static bool RunCommandPromptCommand(string command)
        {
            var processInfo = new ProcessStartInfo
            {
                RedirectStandardOutput = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                Arguments = $"/c {command}",
                FileName = "cmd.exe"
            };
            var process = Process.Start(processInfo);
            string result = process.StandardOutput.ReadToEnd();
            Console.WriteLine(result);
            return process.ExitCode == 0;
        }

        private static bool CreateDirectorySafely(string directory)
        {
            return !Directory.Exists(directory) && Directory.CreateDirectory(directory) != null;
        }

        private static string GetArgumentValue(string[] args)
        {
            return args != null && args.Length > 0
                ? args[0]
                : null;
        }

        private static string GetOptionsValue(string[] args, string option)
        {
            if (args != null)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == option)
                    {
                        return args[i + 1];
                    }
                }
            }

            return null;
        }
    }
}
