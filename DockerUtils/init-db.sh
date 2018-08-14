#!/usr/bin/env bash

: ${SLEEP_LENGTH:=2}

echo Waiting for MSSQL to listen on the default port...

while ! /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P AdminPassword1 -d master -i init-db.sql
do 
    echo MSSQL not ready - sleeping; 
    sleep ${SLEEP_LENGTH}; 
done

echo Waiting for Redis to listen on the default port...

while ! redis-cli ping
do 
    echo Redis not ready - sleeping; 
    sleep ${SLEEP_LENGTH}; 
done
