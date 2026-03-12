#!/bin/bash

# Start SQL Server in the background
/opt/mssql/bin/sqlservr &
SQL_PID=$!

echo "Waiting for SQL Server to start..."
until /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -Q "SELECT 1" -No -C &>/dev/null; do
  echo "  Not ready yet — retrying in 2s..."
  sleep 2
done

echo "SQL Server is up. Running InitDb.sql..."
/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$MSSQL_SA_PASSWORD" -i /init/InitDb.sql -No -C
echo "Databases initialized."

# Hand off to SQL Server process (keep container alive)
wait $SQL_PID

