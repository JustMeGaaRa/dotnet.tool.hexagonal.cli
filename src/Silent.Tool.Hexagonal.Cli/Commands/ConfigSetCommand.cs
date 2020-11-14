using CliFx;
using CliFx.Attributes;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Silent.Tool.Hexagonal.Cli.Infrastructure.Options;
using System;
using System.IO;
using System.Reflection;
using System.Security;
using System.Threading.Tasks;

namespace Silent.Tool.Hexagonal.Cli.Commands
{
    [Command("config set")]
    public class ConfigSetCommand : ICommand
    {
        private readonly IConfiguration _configuration;
        private readonly JsonSerializerSettings _jsonSettings;

        public ConfigSetCommand(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        [CommandOption("--global", 'g')]
        public bool Global { get; set; }

        [CommandOption("--key", 'k', IsRequired = true)]
        public string Key { get; set; }

        [CommandOption("--value", 'v', IsRequired = true)]
        public string Value { get; set; }

        public ValueTask ExecuteAsync(IConsole console)
        {
            var previousValue = _configuration[Key];

            try
            {
                var executingAssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var filePath = Global
                    ? Path.Combine(executingAssemblyPath, Constants.GlobalFileName)
                    : Path.Combine(Directory.GetCurrentDirectory(), Constants.ConfigFileName);

                var generalSection = new GeneralSection();
                _configuration[Key] = Value;
                _configuration.Bind(generalSection);

                var json = JsonConvert.SerializeObject(generalSection, _jsonSettings);
                File.WriteAllText(filePath, json);
                console.Output.WriteLine($"The setting '{Key}' was updated to a value '{Value}' successfully.");
            }
            catch (PathTooLongException ex)
            {
                _configuration[Key] = previousValue;
                console.Output.WriteLine($"The path to the file is too long. Could not save the settings. Error: {ex}");
            }
            catch (UnauthorizedAccessException ex)
            {
                _configuration[Key] = previousValue;
                console.Output.WriteLine($"The access to the file is unauthorized to this app. Error: {ex}");
            }
            catch (SecurityException ex)
            {
                _configuration[Key] = previousValue;
                console.Output.WriteLine($"The security issue occured when trying to access the file. Error: {ex}");
            }

            return default;
        }
    }
}
