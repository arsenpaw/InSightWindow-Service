# Stage 1: Build the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /source

# Copy the solution file and restore dependencies as a separate layer
COPY *.sln .
COPY InSightWindowAPI/*.csproj ./InSightWindowAPI/
RUN dotnet restore

# Copy the entire project and build the application
COPY InSightWindowAPI/. ./InSightWindowAPI/
WORKDIR /source/InSightWindowAPI
RUN dotnet publish -c Release -o /app --no-restore

# Stage 2: Create the final image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app .

# Set the entry point to run the application
ENTRYPOINT ["dotnet", "InSightWindowAPI.dll"]
