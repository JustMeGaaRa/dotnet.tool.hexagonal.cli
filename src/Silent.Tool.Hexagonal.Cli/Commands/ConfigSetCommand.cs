using CliFx;
using CliFx.Attributes;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Silent.Tool.Hexagonal.Cli.Infrastructure.Options;
using System;
using System.IO;
using System.Security;
using System.Threading.Tasks;

namespace Silent.Tool.Hexagonal.Cli.Commands
{
    [Command("config set")]
    public class ConfigSetCommand : ICommand
    {
        private readonly IConfiguration _configuration;
        private readonly IOptions<GeneralOptions> _options;
        private readonly JsonSerializerSettings _jsonSettings;

        public ConfigSetCommand(IOptions<GeneralOptions> options, IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
        }

        [CommandOption("--key", 'k', IsRequired = true)]
        public string Key { get; set; }

        [CommandOption("--value", 'v', IsRequired = true)]
        public string Value { get; set; }

        public ValueTask ExecuteAsync(IConsole console)
        {
            var previousValue = _configuration[Key];

            try
            {
                var generalOptions = new GeneralOptions();
                _configuration[Key] = Value;
                _configuration.GetSection(GeneralOptions.Name).Bind(generalOptions);

                var settings = new { General = generalOptions };
                var json = JsonConvert.SerializeObject(settings, _jsonSettings);
                File.WriteAllText("appsettings.json", json);
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
