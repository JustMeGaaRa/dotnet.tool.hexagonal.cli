# dotnet hexa init

## Name

`dotnet hexa init` - Initializes the project with default project folder structure.

## Synopsis

```bash
dotnet hexa init <PROJECT_NAME>
    [-f|--framework <FRAMEWORK>]

dotnet hexa [-h|--help]
```

## Description

Creates the following folder structure:

```
`---+ <PROJECT_NAME>
    |---+ clients
    |---+ docs
    |---+ samples
    |---+ src
    |---+ test
    `---- <PROJECT_NAME>.sln
```

## Arguments

- `PROJECT_NAME` - The name of the service.

## Options

| Name          | Options           | Description   |
|---            |---                |---            |
| `--framework` | net3.1, net5.0    | The target framework for the service  |

## Examples

```bash
dotnet hexa init --name "PROJECT_NAME"

dotnet hexa init --name "PROJECT_NAME" --framework "net5.0"

dotnet hexa init --name "PROJECT_NAME" --framework "net3.1"
```
