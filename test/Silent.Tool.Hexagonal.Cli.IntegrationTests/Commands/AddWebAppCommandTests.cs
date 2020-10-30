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
            var serviceProvider = Startup.CreateServiceProvider<InitializeCommand>();
            var (console, stdOut, _) = VirtualConsole.CreateBuffered();

            var app = new CliApplicationBuilder()
                .AddCommand<InitializeCommand>()
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
            Assert.True(Directory.Exists("eShopOnWeb/src/webapps/eShopOnWeb/eShopOnWeb.Web"));
        }
    }
}
