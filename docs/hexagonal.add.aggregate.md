# dotnet hexa add aggregate

## Name

`dotnet hexa add aggregate` - Add an aggregate to a specified project / container.

## Synopsis

```bash
dotnet hexa add aggregate <AGGREGATE_NAME>

dotnet hexa add aggregate --help
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

- `AGGREGATE_NAME`

## Examples

```bash
dotnet hexa add aggregate "AGGREGATE_NAME" --service "SERVICE_NAME"
```
