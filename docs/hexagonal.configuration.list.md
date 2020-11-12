# hexa config set

## Name

`hexa config list` - Lists down all the options as a key-value pair.

## Synopsis

```bash
hexa config list
```

## Description

The command lists down all the options that can be modified, so that you know what are the keys and current values.

```bash
[ projects:webapp:template, {COMPANY_NAME}.{WEBAPP_NAME}.{PROJECT_TYPE} ]
[ projects:webapp:path, src/webapps/{WEBAPP_NAME} ]
[ projects:webapp:generate, True ]
[ projects:unitTest:template, {COMPANY_NAME}.{SERVICE_NAME}.{PROJECT_TYPE} ]
[ projects:unitTest:path, test ]
[ projects:unitTest:generate, True ]
[ projects:service:template, {COMPANY_NAME}.{SERVICE_NAME}.{PROJECT_TYPE} ]
[ projects:service:path, src/services/{SERVICE_NAME} ]
[ projects:service:generate, True ]
[ projects:client:template, {COMPANY_NAME}.{SERVICE_NAME}.{PROJECT_TYPE} ]
[ projects:client:path, clients/{SERVICE_NAME} ]
[ projects:client:generate, True ]
[ framework:default, net5.0 ]
[ folders:webappsFolder, webapps ]
[ folders:servicesFolder, services ]
[ folders:samplesFolder, samples ]
[ folders:docsFolder, docs ]
[ folders:clientsFolder, clients ]
```

### Existing Tokens

- `{COMPANY_NAME}` - A company name that will be passed as a command parameter.
- `{SERVICE_NAME}` - A service name that will be passed as a command parameter.
- `{WEBAPP_NAME}` - A web app name that will be passed as a command parameter.
- `{PROJECT_TYPE}` - A project type that will be substituted automatically depending on the project that is generated. Supported types: Domain, Infrastructure, Api, Web, Tests, IntegrationTests, Client.

## Examples

```bash
hexa config list
```
