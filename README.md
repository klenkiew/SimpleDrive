# Simple Drive

This is a simple application which can be used to store files, edit them and share with other users. 
It's been built using Angular 5 and .NET Core 2 as the main technology stack.
The main goal of this project was to learn some new stuff and have fun coding, so the functionality is pretty basic
and the usability somewhat questionable.

## How to build

This application runs on Linux and utilizes Docker containers for building and deployment. 
It requires Docker and Docker Compose installed. It can be run without Docker as well, but it requires installing a few
tools (notably Redis & Postgres) so using Docker is the preferred way.

Full build & deploy from scratch script for Arch-based distros:
```shell
sudo pacman -S docker docker-compose
mkdir SimpleDrive
cd SimpleDrive
git clone https://github.com/klenkiew/SimpleDrive
docker-compose up

firefox localhost:4200
```


 