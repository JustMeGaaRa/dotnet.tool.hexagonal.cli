# hexa config set

## Name

`hexa config set` - Sets the specified value as default one to use in other commands.

## Synopsis

```bash
hexa config set
    [-k|--key <KEY>]
    [-v|--value <VALUE>]

hexa config set [-h|--help]
```

## Description

The tool supports a small set of configurable options for those who want some minor variations for the commonly used standards. The configuration supports the following parameters to change:

```json
{
    "general": {
        "framework": {
            "default": "net5.0"
        },
        "folders": {
            "clientsFolder": "clients",
            "docsFolder": "docs",
            "samplesFolder": "samples",
            "servicesFolder": "services",
            "webappsFolder": "webapps",
        }
    }
}
```

## Options

| Name      | Options   | Required  | Description                           |
|---        |---        |---        |---                                    |
| `--key`   | string    | true      | The target framework for the service  |
| `--value` | string    | true      | Shows list of arguments and options for current command   |

## Examples

```bash
hexa config set --key "general:framework:default" --value "netcoreapp3.1"

hexa config set -k "general:folders:docsFolder" -value "documents"
```
