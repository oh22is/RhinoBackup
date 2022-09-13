-- This is auto-generated code
CREATE VIEW NycYellowTaxi AS
SELECT *
FROM
    OPENROWSET(
        BULK 'https://azureopendatastorage.blob.core.windows.net/nyctlc/yellow/puYear=*/puMonth=*/*.parquet',
        FORMAT = 'parquet'
    ) AS [result];
