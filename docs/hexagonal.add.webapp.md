# hexa add webapp

## Name

`hexa add webapp` - Adds the ASP.NET Core MVC project to the solution using predefined project folder structure.

## Synopsis

```bash
hexa add webapp <WEBAPP_NAME>
    [-f|--framework <FRAMEWORK>]
    [-c|--company <COMPANY_NAME>]

hexa add webapp [-h|--help]
```

## Description

Creates the following folder structure by default:

```
`---+ <PROJECT_NAME>
    |---+ src
        `---+ webapps
            `---+ <WEBAPP_NAME>
                |--+ <WEBAPP_NAME>.Web
                `--+ <WEBAPP_NAME>.Infrastructure
```

If you want to change the folder structure, you can do so by changing the project path and name templates in default configuration file.

## Arguments

- `WEBAPP_NAME` - A name for the web application project.

## Options

| Name          | Options               | Description                           |
|---            |---                    |---                                    |
| `--framework` | netcoreapp3.1, net5.0 | The target framework for the webapp   |
| `--help`      | -                     | Shows list of arguments and options for current command   |

## Examples

```bash
hexa add webapp "Firefox"

hexa add webapp "Firefox" --company "Mozilla" --framework "net5.0"

hexa add webapp --help
```
