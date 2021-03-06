# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY *.csproj .
RUN dotnet restore

# Copy everything else and build
COPY bin/Release/net6.0/publish/ .
RUN dotnet build -c Release
RUN dotnet publish -c Release -o /dist

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0

WORKDIR /app
ENV ASPNETCORE_ENVIROMENT Production
ENV ASPNETCORE_URLS http://+:80
EXPOSE 80

COPY --from=build-env /dist .
ENTRYPOINT ["dotnet", "podcasttranscription.dll"]
