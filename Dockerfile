# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)

# Stage 1: Build the Angular app
FROM node:22.14.0  AS build_node
WORKDIR /app
copy . .
RUN rm -rf node_modules
RUN rm package-lock.json
RUN npm cache clean --force
RUN npm install
RUN npm run build:prod

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0-noble AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0-noble AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CBAS.csproj", "."]
RUN dotnet restore "./CBAS.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./CBAS.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CBAS.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=build_node ./app/wwwroot ./wwwroot
ENTRYPOINT ["dotnet", "CBAS.dll"]



### Install npm dependencies
#FROM node:12.22.12 AS build
##WORKDIR /app/ClientApp
##COPY package*.json ./
#RUN npm install
#COPY package*.json ./
#RUN npm install
#
## Use the official .NET SDK image as the base image
#FROM mcr.microsoft.com/dotnet/sdk:8.0
#
## Set the working directory in the container
#WORKDIR /app
#
## Copy the .csproj file and restore dependencies
#COPY *.csproj ./
#RUN dotnet restore
#
## Copy the rest of the application code
#COPY . .
#
### Install Node.js and npm
##RUN apt-get update && \
    ##apt-get install -y curl gnupg && \
    ##curl -sL https://deb.nodesource.com/setup_12.x | bash - && \
    ##apt-get install -y nodejs
##
### Install Angular CLI globally
##RUN npm install -g @angular/cli
##
## Set the environment to Development
#ENV ASPNETCORE_ENVIRONMENT=Development
#
#
#
## Set the working directory back to the app root
#WORKDIR /app
#
## Expose the port the app runs on
#EXPOSE 5000
#
## Set the entry point to start the .NET application
#ENTRYPOINT ["dotnet", "watch", "run"]
#
