# Rhino Backup

```shell
********************************************************************************
********************************************************************************
********************************************************************************
********************************************************************************
***************************,****************************************************
**************@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@#****************************
*****************%@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@/**********************
**********************%@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@*******************
*************************#@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@*****************
***************************(@@@@**@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@***************
*********************************@***@@@@@@@@@@@@@@@@@@@@@@@@@@@@@**************
**********************************@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@*************
***********************************#@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@%************
*****************************#@****@@@@@**@@@@@@@@@@@@@@@@@@@@@@@@@@************
*****************************@@/*********@@@@@@@@@@**@@@@@@@@@@@@@@@************
****************************************@@@@@&*****@@@@@@@@@@@@@@@@(************
************************************************@@@@@@@@@@@@@@@@@@@*************
*******************************************#@@@@@@@@@@@@@@@@@@@@@@**************
*****************/@@@@@@@@@****************@@@@@@@@@@@@@@@@@@@@@****************
***************#@@@@@@@@@@@@@@@&*********&@@@@@@@@@@@@@@@@@@@@#*****************
**************@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@********************
**************@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@************************
**************@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@*****************************
**************@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@***************************
**************@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@**************************
**************@@@@@@@@@@@@@@@@@@@@&@@@@@@@@@@@@@@@@@@@@&************************
**************@@@@@@@@@@@@@@@@@@@@&*@@@@@@@@@@@@@@@@@@@@@***********************
**************@@@@@@@@@@@@@@@@@@@@&***@@@@@@@@@@@@@@@@@@@@%*********************
**************@@@@@@@@@@@@@@@@@@@@&****@@@@@@@@@@@@@@@@@@@@@********************
**************@@@@@@@@@@@@@@@@@@@@&******@@@@@@@@@@@@@@@@@@@@(******************
**************@@@@@@@@@@@@@@@@@@@@&*******@@@@@@@@@@@@@@@@@@@@@*****************
**************@@@@@@@@@@@@@@@@@@@@&********/@@@@@@@@@@@@@@@@@@@@/***************
**************@@@@@@@@@@@@@@@@@@@@&**********@@@@@@@@@@@@@@@@@@@@@**************
**************@@@@@@@@@@@@@@@@@@@@&***********%@@@@@@@@@@@@@@@@@@@@*************
********************************************************************************
********************************************************************************
********************************************************************************
```

RhinoBackup was developed by our team within oh22 based on our experience in various Azure Synapse Analytics projects.

We would be happy if our solution is used in other projects and we can incorporate the collected experiences, improvement suggestions and new requirements into one of the next versions.

## App Start

Use either `--export` or `--import`

Optional:

Definition of output types, which are mutual exclusive:

- `--AsDirectory={Path}` -> defaults to `export`
- `--AsFile={Path}`

Example:

`dotnet run --export`

## Config Options

### MsSql Config Settings

Configures where to Export or Import from

```jsonc
{
  "Sql": {
    "SqlConnection": "", // Connection String, Either this or rest
    "SqlEndpoint": "", //Host
    "Database": "", 
    "Username": "",
    "Password": ""
  }
}
```

### Options Settings

Configures what to Export

```jsonc
{
  "Options": {
    "DoExternalDataSources": true,
    "DoExternalFileFormats": true,
    "DoExternalTables": true,
    "DoFunctions": true,
    "DoSchemas": true,
    "DoStoredProcedures": true,
    "DoViews": true,
    "AreSystemObjects": false,
    "ExcludedSchemas": [] // String Array of SchemaNames
  }
}
```

### Serilog Settings

Included Sinks: `Console` and `File`

Example:

```jsonc
{
  "Serilog": {
    "MinimumLevel": "Debug",
    "WriteTo": [
      { "Name": "Console" }
    ]
  }
}
```

[Read Further](https://github.com/serilog/serilog-settings-configuration)

## Config Types

The config can be set in different stages. They are consecutive and if present override the previous stage.

appsettings.json > appsettings.{Environment}.json > Environment Variables > CommandLine

### Dotnet Config

Use `appsettings.json` or `appsettings.{Environment}.json`

```jsonc
{
  "Sql": {/*...*/}, // MsSql Settings
  "Options": {/*...*/}, // Options Settings
  "SeriLog": {/*...*/} // Serilog Settings
}
```

[Read Further](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-6.0)

### Environment Config

Config can also be achieved or overridden through environment variables prefixed with `RHINO_`

Eg. `RHINO_sql:Database=Test` or `RHINO_Serilog:MinimumLevel=Information`

### Command Line Config

Config can also be achieved or overridden through command line.

Eg. `--sql:Password=P4$$w0rd` or `--Options:ExcludedSchemas:0=obsolete`
