using CliFx;
using CliFx.Attributes;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace Silent.Tool.Hexagonal.Cli.Commands
{
    [Command("config set")]
    public class ConfigSetCommand : ICommand
    {
        private readonly IConfiguration _configuration;

        public ConfigSetCommand(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [CommandOption("--key", 'k', IsRequired = true)]
        public string Key { get; set; }

        [CommandOption("--value", 'v', IsRequired = true)]
        public string Value { get; set; }

        public ValueTask ExecuteAsync(IConsole console)
        {
            _configuration[Key] = Value;
            return default;
        }
    }
}
