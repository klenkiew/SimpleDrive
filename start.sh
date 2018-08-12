#!/usr/bin/env bash

docker-compose run --rm wait-for-deps
docker-compose up --scale wait-for-deps=0