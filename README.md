# .NET Tool for Clean Architecture Solutions
A .NET CLI tool for creating and managing projects in a folder structure suitable for Clean Architecture and Microservices. Is similar to how Angular CLI tool uses commands to create components, modules, services, etc.

## Install

- `dotnet tool install Silent.Tool.Hexagonal.Cli --version 1.0.0`

## Features
- Can create a general-purpose solution folder structure for Clean Architecture projects
- Can add a RESTful service with empty dependencies according to Clean Architecture
- Can add components to the service according to Domain-Driven Design

## Usage
- [dotnet hexa](https://github.com/JustMeGaaRa/dotnet.tool.hexagonal.cli/blob/main/docs/hexagonal.init.md) - Shows help information and a list of commands.
- [dotnet hexa config](https://github.com/JustMeGaaRa/dotnet.tool.hexagonal.cli/blob/main/docs/hexagonal.configuration.md) - Gets or sets the configuration values for the tool.
- [dotnet hexa init](https://github.com/JustMeGaaRa/dotnet.tool.hexagonal.cli/blob/main/docs/hexagonal.init.md) - Initializes the project with default project folder structure.
- [dotnet hexa add service](https://github.com/JustMeGaaRa/dotnet.tool.hexagonal.cli/blob/main/docs/hexagonal.add.service.md) - Adds a ASP.NET Core project type with all the dependencies.
- [dotnet hexa add webapp](https://github.com/JustMeGaaRa/dotnet.tool.hexagonal.cli/blob/main/docs/hexagonal.add.webapp.md) - Adds a ASP.NET Core MVC project type with all the dependencies.
- [dotnet hexa add aggregate](https://github.com/JustMeGaaRa/dotnet.tool.hexagonal.cli/blob/main/docs/hexagonal.add.aggregate.md) - Adds an aggregate entity to the project.