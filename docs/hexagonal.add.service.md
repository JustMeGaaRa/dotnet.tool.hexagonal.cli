# dotnet hexa add service

## Name

`dotnet hexa add service` - Adds the ASP.NET Core project to the solution using predefined project folder structure.

## Synopsis

```bash
dotnet hexa add service <SERVICE_NAME>
    [-f|--framework <FRAMEWORK>]

dotnet hexa add service [-h|--help]
```

## Description

Creates the following folder structure:

```
`---+ <PROJECT_NAME>
    |---+ clients
    |   +---+ <SERVICE_NAME>
    |       |---+ <SERVICE_NAME>.Abstracts
    |       `---+ <SERVICE_NAME>.Client
    |---+ src
    |   `---+ services
    |       `---+ <SERVICE_NAME>
    |           |---+ <SERVICE_NAME>.Api
    |           |---+ <SERVICE_NAME>.Domain
    |           `---+ <SERVICE_NAME>.Infrastructure
    |---+ test
        `---+ <SERVICE_NAME>.Domain.Tests
```

## Arguments

- `SERVICE_NAME`

## Options

| Name          | Options   | Description                           |
|---            |---        |---                                    |
| `--framework` | string    | The target framework for the service  |
| `--help`      | -         | Shows list of arguments and options for current command   |

## Examples

```bash
dotnet hexa add service "SERVICE_NAME"

dotnet hexa add service "SERVICE_NAME" --framework "net5.0"

dotnet hexa add service --help
```
