# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY InSightWindowAPI/*.csproj ./InSightWindowAPI/
RUN dotnet restore

# copy everything else and build app
COPY InSightWindowAPI/. ./InSightWindowAPI/
WORKDIR /source/InSightWindowAPI
RUN dotnet publish  D:\InSightWindow Server\InSightWindow-Service\InSightWindowAPI\InSightWindowAPI.sln -c release -o /app --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "aspnetapp.dll"]