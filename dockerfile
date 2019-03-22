FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /src

# copy csproj and restore as distinct layers
RUN mkdir WebAppServer

COPY WebAppServer/WebAppServer.csproj ./WebAppServer/WebAppServer.csproj

WORKDIR /src/WebAppServer
RUN dotnet restore

WORKDIR /src

# copy everything else and build app
COPY WebAppServer/. ./WebAppServer/
WORKDIR /src/WebAppServer
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2

WORKDIR /app

RUN mkdir -p movies
RUN mkdir -p series
RUN mkdir -p save
RUN mkdir -p config
RUN mkdir -p animes
RUN mkdir -p Database

COPY WebAppServer/Database/app.db ./Database/app.db
VOLUME ./Database

COPY --from=build-env /src/WebAppServer/out .

EXPOSE 80
ENTRYPOINT ["dotnet", "WebAppServer.dll"]