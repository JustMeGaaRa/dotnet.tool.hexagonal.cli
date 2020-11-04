# hexa add webapp

## Name

`hexa add webapp` - Adds the ASP.NET Core MVC project to the solution using predefined project folder structure.

## Synopsis

```bash
hexa add webapp <WEBAPP_NAME>
    [-f|--framework <FRAMEWORK>]

hexa add webapp [-h|--help]
```

## Description

Creates the following folder structure:

```
`---+ <PROJECT_NAME>
    |---+ src
        `---+ webapps
            `---+ <WEBAPP_NAME>
                |--+ <WEBAPP_NAME>.Web
                `--+ <WEBAPP_NAME>.Infrastructure
```

## Arguments

- `WEBAPP_NAME`

## Options

| Name          | Options               | Description                           |
|---            |---                    |---                                    |
| `--framework` | netcoreapp3.1, net5.0 | The target framework for the webapp   |
| `--help`      | -                     | Shows list of arguments and options for current command   |

## Examples

```bash
hexa add webapp "WEBAPP_NAME"

hexa add webapp "WEBAPP_NAME" --framework "net5.0"

hexa add webapp --help
```
