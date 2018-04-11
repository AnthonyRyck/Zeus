FROM microsoft/aspnetcore-build:2.0 AS build-env
WORKDIR /src

# copy csproj and restore as distinct layers
RUN mkdir MoviesLib
RUN mkdir WebAppServer

COPY MoviesLib/MoviesLib.csproj ./MoviesLib/MoviesLib.csproj
COPY WebAppServer/WebAppServer.csproj ./WebAppServer/WebAppServer.csproj

WORKDIR /src/MoviesLib
RUN dotnet restore

WORKDIR /src/WebAppServer
RUN dotnet restore

WORKDIR /src

# copy everything else and build app
COPY MoviesLib/. ./MoviesLib/
COPY WebAppServer/. ./WebAppServer/
WORKDIR /src/WebAppServer
RUN dotnet publish -c Release -o out

FROM microsoft/aspnetcore:2.0

WORKDIR /app

RUN mkdir -p movies
RUN mkdir -p save
RUN mkdir -p config

COPY --from=build-env /src/WebAppServer/out .
EXPOSE 80
ENTRYPOINT ["dotnet", "WebAppServer.dll"]