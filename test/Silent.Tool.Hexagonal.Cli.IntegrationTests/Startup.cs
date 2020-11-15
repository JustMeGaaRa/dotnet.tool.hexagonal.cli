﻿using CliFx;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Silent.Tool.Hexagonal.Cli.Infrastructure.Options;
using System;

namespace Silent.Tool.Hexagonal.Cli.IntegrationTests
{
    public class Startup
    {
        public static IServiceProvider CreateServiceProvider<TCommand>() where TCommand : class, ICommand
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var services = new ServiceCollection()
                .AddOptions()
                .Configure<GeneralSection>(config)
                .AddSingleton<IConfiguration>(config)
                .AddTransient<TCommand>();

            return services.BuildServiceProvider();
        }
    }
}
