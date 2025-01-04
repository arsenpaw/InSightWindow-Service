# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.sln .
COPY InSightWindowAPI/*.csproj ./InSightWindowAPI/


# copy everything else and build app
COPY InSightWindowAPI/. ./InSightWindowAPI/
WORKDIR /source/InSightWindowAPI
RUN dotnet nuget locals all --clear
RUN dotnet restore
RUN dotnet publish -c release -o /app --no-restore

RUN mkdir -p app/certificates
COPY Certificates/aspnetapp.pfx /app/certificates/

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=build /app ./

ENTRYPOINT ["dotnet", "InSightWindowAPI.dll"]