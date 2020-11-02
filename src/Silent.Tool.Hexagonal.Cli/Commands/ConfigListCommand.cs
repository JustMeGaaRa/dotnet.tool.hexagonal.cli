using CliFx;
using CliFx.Attributes;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace Silent.Tool.Hexagonal.Cli.Commands
{
    [Command("config list")]
    public class ConfigListCommand : ICommand
    {
        private readonly IConfiguration _configuration;

        public ConfigListCommand(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public ValueTask ExecuteAsync(IConsole console)
        {
            _configuration
                .AsEnumerable()
                .Where(setting => setting.Value != null)
                .ToList()
                .ForEach(setting => console.Output.WriteLine($"[{setting.Key}, {setting.Value}]"));

            return default;
        }
    }
}
