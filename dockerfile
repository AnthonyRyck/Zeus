FROM microsoft/aspnetcore-build:2.0 AS build-env
WORKDIR /src

# copy csproj and restore as distinct layers
RUN mkdir WebAppServer
RUN mkdir TMDbLib

COPY WebAppServer/WebAppServer.csproj ./WebAppServer/WebAppServer.csproj
COPY TMDbLib/TMDbLib.csproj ./TMDbLib/TMDbLib.csproj

WORKDIR /src/TMDbLib
RUN dotnet restore

WORKDIR /src/WebAppServer
RUN dotnet restore

WORKDIR /src

# copy everything else and build app
COPY TMDbLib/. ./TMDbLib/
COPY WebAppServer/. ./WebAppServer/
WORKDIR /src/WebAppServer
RUN dotnet publish -c Release -o out

FROM microsoft/aspnetcore:2.0

WORKDIR /app

RUN mkdir -p movies
RUN mkdir -p series
RUN mkdir -p save
RUN mkdir -p config
RUN mkdir -p animes

COPY --from=build-env /src/WebAppServer/out .
EXPOSE 80
ENTRYPOINT ["dotnet", "WebAppServer.dll"]