using CliFx;
using CliFx.Attributes;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Silent.Tool.Hexagonal.Cli.Commands
{
    [Command("config list")]
    public class ConfigListCommand : ICommand
    {
        [CommandOption("--global", 'g')]
        public bool Global { get; set; }

        public ValueTask ExecuteAsync(IConsole console)
        {
            var executingAssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var globalFilePath = Path.Combine(executingAssemblyPath, Constants.GlobalFileName);
            var localFilePath = Path.Combine(Directory.GetCurrentDirectory(), Constants.ConfigFileName);

            var appsettingsFilePath = Global || !File.Exists(localFilePath)
                ? globalFilePath
                : localFilePath;

            var config = new ConfigurationBuilder()
                .AddJsonFile(appsettingsFilePath, true, true)
                .Build();

            config
                .AsEnumerable()
                .Where(setting => setting.Value != null)
                .ToList()
                .ForEach(setting => console.Output.WriteLine($"[{setting.Key}, {setting.Value}]"));

            return default;
        }
    }
}
