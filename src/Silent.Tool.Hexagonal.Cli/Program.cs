using CliFx;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Silent.Tool.Hexagonal.Cli.Commands;
using Silent.Tool.Hexagonal.Cli.Infrastructure.Options;
using System.Threading.Tasks;

namespace Silent.Tool.Hexagonal.Cli
{
    internal class Program
    {
        private static async Task<int> Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var services = new ServiceCollection()
                .AddOptions()
                .Configure<GeneralOptions>(config.GetSection(GeneralOptions.Name))
                .AddSingleton<IConfiguration>(config)
                .AddTransient<InitializeCommand>()
                .AddTransient<AddServiceCommand>()
                .AddTransient<AddWebAppCommand>()
                .AddTransient<ConfigSetCommand>()
                .AddTransient<ConfigListCommand>();

            using var serviceProvider = services.BuildServiceProvider();

            return await new CliApplicationBuilder()
                .AddCommandsFromThisAssembly()
                .UseTypeActivator(serviceProvider.GetService)
                .Build()
                .RunAsync();
        }
    }
}
