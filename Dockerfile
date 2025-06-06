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
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
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
