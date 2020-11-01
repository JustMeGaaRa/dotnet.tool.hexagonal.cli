using CliFx;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Silent.Tool.Hexagonal.Cli.IntegrationTests.Commands
{
    public class AddServiceCommandTests
    {
        [Fact]
        public async void RunAsync_ShouldCreateService()
        {
            // Arrange
            var serviceProvider = Startup.CreateServiceProvider<AddServiceCommand>();
            var (console, stdOut, _) = VirtualConsole.CreateBuffered();

            var app = new CliApplicationBuilder()
                .AddCommand<AddServiceCommand>()
                .UseTypeActivator(serviceProvider.GetService)
                .UseConsole(console)
                .Build();

            // add service "Orders" --framework "net5.0"
            // add service --help
            var args = new[] { "add", "service", "Orders", "--framework", "net5.0" };
            var envVars = new Dictionary<string, string>();

            // Act
            await app.RunAsync(args, envVars);

            // Assert
            Assert.True(Directory.Exists("src/services/Orders/Orders.Api"));
            Assert.True(Directory.Exists("src/services/Orders/Orders.Domain"));
            Assert.True(Directory.Exists("src/services/Orders/Orders.Infrastructure"));
            Assert.True(Directory.Exists("test/Orders.Domain.Tests"));

            // Cleanup
            if (Directory.Exists("src")) Directory.Delete("src", true);
            if (Directory.Exists("test")) Directory.Delete("test", true);
        }
    }
}
