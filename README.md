# Dotnet API with docker

## Getting Started

### Without using Docker.

Used db : PostgreSQL
Before starting set up PostgreSQL and change pgsql settings in "appsettings.json" (production settings) and "appsettings.Development.json" (development settings)

First time run the below command for the database migartion
```sh
$ dotnet ef migrations add MyFirstMigration
$ dotnet ef database update
```

Now run the application using below command
```sh
$ dotnet run
```

### Using Docker

Step 1: Build the application in release mode using below command
```sh
$ dotnet build -c Release 
$ dotnet publish -c Release /p:UseAppHost=false
```
One thing to notice here is, inside the "appsettings.json" file, the value of "DefaultConnection" uses "db" as host. This name we have to give to the pgsql in Docker compose.

Step 2: Go to the publish folder; example => ./bin/Release/net7.0/publish/ ; Create a file named "Dockerfile" and paste below code inside that file
```sh
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY . .

ENTRYPOINT ["dotnet", "BlogApp.dll"]
```

Step 3: Build the docker file using below command inside the pulish folder
```sh
$ docker build -t devaliakbar/testapi . 
```
Here "devaliakbar/testapi" is name which given to the docker image. we can use this name to run this image.

Step 4: Compose the docker file for running the app; for example, create a file anywhere with name "DockerComposeExample.yaml" and paste below code
```sh
version: '3.8'
services:
  db:
    image: postgres
    container_name: pgsql-dev
    restart: always
    environment:
      - POSTGRES_USER=username
      - POSTGRES_PASSWORD=password
      - POSTGRES_DB=blogapp
    ports:
      - 5432:5432
    volumes: 
      - db:/var/lib/postgresql/data
  testserver:
    image: devaliakbar/blogapp
    container_name: blogapp
    restart: always
    ports:
      - 5000:80
volumes:
  db:
    driver: local
```

Step 5: Now run the docker-compose up command
```sh
$ docker-compose -f DockerComposeExample.yaml up  
```
Now PostgreSQL and our app is running.

Step 6: For the fist time we have to migrate the db. for that go to our project folder and run migartion command
```sh
$ dotnet ef migrations add MyFirstMigration
$ dotnet ef database update
```