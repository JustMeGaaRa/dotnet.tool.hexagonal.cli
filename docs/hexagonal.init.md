# hexa init

## Name

`hexa init` - Initializes the project with default project folder structure.

## Synopsis

```bash
hexa init <PROJECT_NAME>
```

## Description

Creates the following folder structure:

```
`---+ <PROJECT_NAME>
    |---- <PROJECT_NAME>.sln
    `---- hexaconfig.json
```

## Arguments

- `PROJECT_NAME` - The name of the service.

## Examples

```bash
hexa init "Mozilla.Firefox"
```
