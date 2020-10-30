# dotnet hexa add component

## Name

`dotnet hexa add component` - Add a component to a specified project / container.

## Synopsis

```bash
dotnet hexa add component <COMPONENT_NAME>

dotnet hexa add component --help
```

## Description

```
`---+ <PROJECT_NAME>
    `---+ src
        `---+ services
            `---+ <SERVICE_NAME>
                |---+ <SERVICE_NAME>.Api
                |---+ <SERVICE_NAME>.Domain
                `---+ <SERVICE_NAME>.Infrastructure
```

## Arguments

- `COMPONENT_NAME`

## Examples

```bash
dotnet hexa add component "COMPONENT_NAME" --service "SERVICE_NAME"
```
