using CliFx;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Silent.Tool.Hexagonal.Cli.IntegrationTests.Commands
{
    public class InitializeCommandTests
    {
        [Fact]
        public async void RunAsync_ShouldCreateFolderStructure()
        {
            // Arrange
            var serviceProvider = Startup.CreateServiceProvider<InitializeCommand>();
            var (console, stdOut, _) = VirtualConsole.CreateBuffered();

            var app = new CliApplicationBuilder()
                .AddCommand<InitializeCommand>()
                .UseTypeActivator(serviceProvider.GetService)
                .UseConsole(console)
                .Build();

            // init "eShopOnWeb"
            var args = new[] { "init", "eShopOnWeb" };
            var envVars = new Dictionary<string, string>();

            // Act
            await app.RunAsync(args, envVars);

            // Assert
            Assert.True(Directory.Exists("eShopOnWeb"));
            Assert.True(Directory.Exists("eShopOnWeb/src"));
            Assert.True(Directory.Exists("eShopOnWeb/test"));

            // Cleanup
            if (Directory.Exists("eShopOnWeb")) Directory.Delete("eShopOnWeb", true);
        }
    }
}
