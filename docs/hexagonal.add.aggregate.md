# hexa add aggregate

## Name

`hexa add aggregate` - Add an aggregate to a specified project / container.

## Synopsis

```bash
hexa add aggregate <AGGREGATE_NAME>

hexa add aggregate --help
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
hexa add aggregate "AGGREGATE_NAME" --service "SERVICE_NAME"
```
