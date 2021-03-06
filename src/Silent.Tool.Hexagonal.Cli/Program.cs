﻿using CliFx;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Silent.Tool.Hexagonal.Cli.Commands;
using Silent.Tool.Hexagonal.Cli.Infrastructure.Options;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Silent.Tool.Hexagonal.Cli
{
    internal class Program
    {
        private static async Task<int> Main(string[] args)
        {
            var executingAssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var globalFilePath = Path.Combine(executingAssemblyPath, Constants.GlobalFileName);
            var localFilePath = Path.Combine(Directory.GetCurrentDirectory(), Constants.ConfigFileName);

            var appsettingsFilePath = File.Exists(localFilePath)
                ? localFilePath
                : globalFilePath;

            var config = new ConfigurationBuilder()
                .AddJsonFile(appsettingsFilePath, true, true)
                .Build();

            var services = new ServiceCollection()
                .AddOptions()
                .Configure<GeneralSection>(config)
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
