#!/usr/bin/env bash

: ${SLEEP_LENGTH:=2}

echo Waiting for MSSQL to listen on the default port...

while ! /opt/mssql-tools/bin/sqlcmd -S localhost -U dotnetUser -P Password1 -d UsersDb -i test.sql
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
