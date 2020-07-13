# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /source

# Create folders
RUN mkdir ./TwitchHandler
RUN mkdir ./Shared

# Copy csproj
COPY ./TwitchHandler/TwitchHandler.csproj ./TwitchHandler
COPY ./Shared/Shared.csproj ./Shared

WORKDIR /source/TwitchHandler

# Restore
RUN dotnet restore -r linux-x64

WORKDIR /source

# Copy everything 
COPY ./TwitchHandler ./TwitchHandler
COPY ./Shared ./Shared

WORKDIR /source/TwitchHandler

RUN dotnet publish -c release -o /app -r linux-x64 --self-contained false --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-bionic
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["./TwitchHandler"]