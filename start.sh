#!/usr/bin/env bash

docker-compose run --rm init-db
docker-compose up --scale init-db=0