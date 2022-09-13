# RhinoBackup

## Description

Exports/ imports a Synapse Serverless Database to/ from an output adapter.

## Arguments

| Name                          | Type                 | Description                                         | Default |
| ----------------------------- | -------------------- | --------------------------------------------------- | ------- |
| Sql:SqlConnection             | string               | The connection string.                              |         |
| Sql:SqlEndpoint               | string               | The sql endpoint.                                   |         |
| Sql:Database                  | string               | The name of the database.                           |         |
| Sql:Username                  | string               | The username for the database.                      |         |
| Sql:Password                  | string               | The password for the database.                      |         |
| Options:DoExternalTables      | boolean (true/false) | Should External Tables be exported/ imported.       | false    |
| Options:DoExternalDataSources | boolean (true/false) | Should External Data Sources be exported/ imported. | false    |
| Options:DoExternalFileFormats | boolean (true/false) | Should External File Formats be exported/ imported. | false    |
| Options:DoFunctions           | boolean (true/false) | Should Functions be exported/ imported.             | false    |
| Options:DoSchemas             | boolean (true/false) | Should Schemas be exported/ imported.               | false    |
| Options:DoStoredProcedures    | boolean (true/false) | Should Stored Procedures be exported/ imported.     | false    |
| Options:DoViews               | boolean (true/false) | Should Views be exported/ imported.                 | false    |
| Options:AreSystemObjects      | boolean (true/false) | Should SystemObjects be exported.                   | false   |
| Options:ExcludedSchemas       | array of string      | Which schema should be excluded.                    | []      |

## Exceptions

Either a sql connection string or a sql endpoint, database, username and password must be provided.
If a sql connection string is provided it will be used, even if the rest is also provided.

## Example

```shell
RhinoBackup.exe 
--export
--Sql:SqlConnection="<<ConnectionString>>"
--Options:ExternalTables=false
--Options:ExternalDataSources=false
--Options:ExternalFileFormats=false
--Options:Functions=false
--Options:Schemas=false
--Options:StoredProcedures=false
```

## Impacts

If the import mode will be used, the application will execute the provided SQL scripts on the server.
The export mode will create new data on the output.
