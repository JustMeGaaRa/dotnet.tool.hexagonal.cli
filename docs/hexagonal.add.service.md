# hexa add service

## Name

`hexa add service` - Adds the ASP.NET Core project to the solution using predefined project folder structure.

## Synopsis

```bash
hexa add service <SERVICE_NAME>
    [-f|--framework <FRAMEWORK>]
    [-c|--company <COMPANY_NAME>]

hexa add service [-h|--help]
```

## Description

Creates the following folder structure by default:

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

If you want to change the folder structure, you can do so by changing the project path and name templates in default configuration file.

## Arguments

- `SERVICE_NAME` - A name for the service project.

## Options

| Name          | Options   | Description                           |
|---            |---        |---                                    |
| `--framework` | string    | The target framework for the service  |
| `--help`      | -         | Shows list of arguments and options for current command   |

## Examples

```bash
hexa add service "Firefox"

hexa add service "Firefox" --company "Mozilla" --framework "net5.0"

hexa add service --help
```
