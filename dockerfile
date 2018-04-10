FROM microsoft/aspnetcore-build:2.0 AS build-env
WORKDIR /app

# copy csproj and restore as distinct layers
RUN mkdir MoviesLib
RUN mkdir WebAppServer

COPY MoviesLib/MoviesLib.csproj ./MoviesLib/MoviesLib.csproj
COPY WebAppServer/WebAppServer.csproj ./WebAppServer/WebAppServer.csproj

WORKDIR /app/MoviesLib
RUN dotnet restore

WORKDIR /app/WebAppServer
RUN dotnet restore

WORKDIR /app

# copy everything else and build app
COPY MoviesLib/. ./MoviesLib/
COPY WebAppServer/. ./WebAppServer/
WORKDIR /app/WebAppServer
RUN mkdir out
RUN dotnet publish -o out


FROM microsoft/aspnetcore:2.0

WORKDIR /zeus
RUN mkdir -p movies
RUN mkdir -p save
RUN mkdir -p config

COPY --from=build-env /app/WebAppServer/out .
RUN dir
ENTRYPOINT ["dotnet", "WebAppServer.dll"]