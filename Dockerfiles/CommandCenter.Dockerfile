# https://hub.docker.com/_/microsoft-dotnet-core
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /source

COPY ./CommandCenter ./CommandCenter
COPY ./Shared ./Shared
WORKDIR /source/CommandCenter

RUN dotnet restore -r linux-x64
RUN dotnet publish -c release -o /app -r linux-x64 --self-contained false --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/core/runtime:3.1-bionic
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["./CommandCenter"]