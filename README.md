# .NET Tool for Clean Architecture Solutions
A .NET CLI tool for creating and managing projects in a folder structure suitable for Clean Architecture and Microservices. Is similar to how Angular CLI tool uses commands to create components, modules, services, etc.

## Install

- `dotnet tool install Silent.Tool.Hexagonal.Cli --version 1.0.0`

## Features
- Supports a command for generating a general-purpose solution folder structure for Clean Architecture projects
- Suppotrs commands for generating a RESTful service with empty dependencies according to Clean Architecture
- Supports commands for generating a webapp project
- Automatically adds references to the projects created
- Automatically add references to solution file for generated projects
- Automatically generates the projects for unit and integration testing
- Supports a bit of configuration for project folder names and default framework

## Usage
- [dotnet hexa](https://github.com/JustMeGaaRa/dotnet.tool.hexagonal.cli/blob/main/docs/hexagonal.init.md) - Shows help information and a list of commands.
- [dotnet hexa init](https://github.com/JustMeGaaRa/dotnet.tool.hexagonal.cli/blob/main/docs/hexagonal.init.md) - Initializes the project with default project folder structure.
- [dotnet hexa add service](https://github.com/JustMeGaaRa/dotnet.tool.hexagonal.cli/blob/main/docs/hexagonal.add.service.md) - Adds a ASP.NET Core project type with all the dependencies.
- [dotnet hexa add webapp](https://github.com/JustMeGaaRa/dotnet.tool.hexagonal.cli/blob/main/docs/hexagonal.add.webapp.md) - Adds a ASP.NET Core MVC project type with all the dependencies.
