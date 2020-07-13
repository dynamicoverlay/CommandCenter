# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /source

# Create folders
RUN mkdir ./DiscordHandler
RUN mkdir ./Shared

# Copy csproj
COPY ./DiscordHandler/DiscordHandler.csproj ./DiscordHandler
COPY ./Shared/Shared.csproj ./Shared

WORKDIR /source/DiscordHandler

# Restore
RUN dotnet restore -r linux-x64

WORKDIR /source

# Copy everything 
COPY ./DiscordHandler ./DiscordHandler
COPY ./Shared ./Shared

WORKDIR /source/DiscordHandler

RUN dotnet publish -c release -o /app -r linux-x64 --self-contained false --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-bionic
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["./DiscordHandler"]