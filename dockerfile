FROM microsoft/dotnet:2.0-sdk AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY MoviesLib/*.csproj ./MoviesLib/
COPY WebAppServer/*.csproj ./WebAppServer/
RUN dotnet restore

# copy everything else and build app
COPY MoviesLib/. ./MoviesLib/
COPY WebAppServer/. ./WebAppServer/
WORKDIR /app/WebAppServer
RUN dotnet publish -o out /p:PublishWithAspNetCoreTargetManifest="false"


FROM microsoft/dotnet:2.0-runtime AS runtime

RUN mkdir -p /movies

WORKDIR /zeus
RUN mkdir -p save
RUN mkdir -p config

COPY --from=build /app/WebAppServer/out ./
EXPOSE 80/tcp
ENTRYPOINT ["dotnet", "WebAppServer.dll"]