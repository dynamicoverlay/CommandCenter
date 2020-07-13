# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /source

# Create folders
RUN mkdir ./CommandCenter
RUN mkdir ./Shared

# Copy csproj
COPY ./CommandCenter/CommandCenter.csproj ./CommandCenter
COPY ./Shared/Shared.csproj ./Shared

WORKDIR /source/CommandCenter

# Restore
RUN dotnet restore -r linux-x64

WORKDIR /source

# Copy everything 
COPY ./CommandCenter ./CommandCenter
COPY ./Shared ./Shared

WORKDIR /source/CommandCenter

RUN dotnet publish -c release -o /app -r linux-x64 --self-contained false --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-bionic
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["./CommandCenter"]