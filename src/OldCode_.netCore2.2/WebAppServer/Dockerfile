FROM microsoft/dotnet:2.1-sdk AS build-env
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

FROM microsoft/dotnet:2.1-aspnetcore-runtime

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




#FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
#WORKDIR /app
#EXPOSE 80
#
#FROM microsoft/dotnet:2.1-sdk AS build
#WORKDIR /src
#COPY WebAppServer/WebAppServer.csproj WebAppServer/
#COPY TMDbLib/TMDbLib.csproj TMDbLib/
#RUN dotnet restore WebAppServer/WebAppServer.csproj
#COPY . .
#WORKDIR /src/WebAppServer
#RUN dotnet build WebAppServer.csproj -c Release -o /app
#
#FROM build AS publish
#RUN dotnet publish WebAppServer.csproj -c Release -o /app
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app .
#ENTRYPOINT ["dotnet", "WebAppServer.dll"]
