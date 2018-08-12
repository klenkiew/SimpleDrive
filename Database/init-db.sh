#!/usr/bin/env bash

#wait for the SQL Server to come up
sleep 20s

#run the setup script to create the DB and the schema in the DB
sqlcmd -S localhost -U sa -P AdminPassword1 -d master -i init-db.sql