# hexa init

## Name

`hexa init` - Initializes the project with default project folder structure.

## Synopsis

```bash
hexa init <PROJECT_NAME>
    [-c|--company <COMPANY_NAME>]
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
    |---- <PROJECT_NAME>.sln
    `---- .hexaconfig
```

## Arguments

- `PROJECT_NAME` - The name of the service.

## Examples

```bash
hexa init "Firefox" --company "Mozilla"
```
