﻿using CliFx;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Silent.Tool.Hexagonal.Cli.Commands;
using System.Collections.Generic;
using Xunit;

namespace Silent.Tool.Hexagonal.Cli.IntegrationTests.Commands
{
    public class ConfigSetCommandTests
    {
        [Fact]
        public async void RunAsync_ShouldListTheSettingsInTheOutput()
        {
            // Arrange
            var serviceProvider = Startup.CreateServiceProvider<ConfigSetCommand>();
            var (console, output, _) = VirtualConsole.CreateBuffered();

            var app = new CliApplicationBuilder()
                .AddCommand<ConfigSetCommand>()
                .UseTypeActivator(serviceProvider.GetService)
                .UseConsole(console)
                .Build();

            // config list
            var args = new[] { "config", "set", "--key", "general:framework:default", "--value", "netcoreapp3.1" };
            var envVars = new Dictionary<string, string>();

            // Act
            await app.RunAsync(args, envVars);
            var settings = serviceProvider.GetService<IConfiguration>();

            // Assert
            Assert.Equal("netcoreapp3.1", settings["general:framework:default"]);
        }
    }
}
