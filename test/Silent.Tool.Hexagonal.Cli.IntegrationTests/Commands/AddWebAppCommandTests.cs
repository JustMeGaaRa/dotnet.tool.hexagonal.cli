using CliFx;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Silent.Tool.Hexagonal.Cli.IntegrationTests.Commands
{
    public class AddWebAppCommandTests
    {
        [Fact]
        public async void RunAsync_ShouldCreateWebApp()
        {
            // Arrange
            var serviceProvider = Startup.CreateServiceProvider<AddWebAppCommand>();
            var (console, output, _) = VirtualConsole.CreateBuffered();

            var app = new CliApplicationBuilder()
                .AddCommand<AddWebAppCommand>()
                .UseTypeActivator(serviceProvider.GetService)
                .UseConsole(console)
                .Build();

            // add webapp "eShopOnWeb" --framework "net5.0"
            // add webapp --help
            var args = new[] { "add", "webapp", "eShopOnWeb", "--framework", "net5.0" };
            var envVars = new Dictionary<string, string>();

            // Act
            await app.RunAsync(args, envVars);

            // Assert
            Assert.True(Directory.Exists("src/webapps/eShopOnWeb/eShopOnWeb.Web"));

            // Cleanup
            if (Directory.Exists("src")) Directory.Delete("src", true);
        }
    }
}
