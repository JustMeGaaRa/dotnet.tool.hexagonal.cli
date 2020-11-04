# hexa add service

## Name

`hexa add service` - Adds the ASP.NET Core project to the solution using predefined project folder structure.

## Synopsis

```bash
hexa add service <SERVICE_NAME>
    [-f|--framework <FRAMEWORK>]

hexa add service [-h|--help]
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
hexa add service "SERVICE_NAME"

hexa add service "SERVICE_NAME" --framework "net5.0"

hexa add service --help
```
