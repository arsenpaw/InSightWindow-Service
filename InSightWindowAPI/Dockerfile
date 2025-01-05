# Use the official .NET SDK image for the build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy the entire solution and project files to the build context
COPY . ./
ARG FIREBASE_KEY_PATH
ENV GOOGLE_APPLICATION_CREDENTIALS=$FIREBASE_KEY_PATH
# Restore NuGet packages and build the application
RUN dotnet restore
RUN dotnet publish -c Release -o /app

# Use the official .NET runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy the published output from the build stage
COPY --from=build /app ./

ARG FIREBASE_KEY_PATH
COPY --from=build $FIREBASE_KEY_PATH /app/firebase-admin-sdk.json

# Set the environment variable for Firebase
ENV GOOGLE_APPLICATION_CREDENTIALS /app/firebase-admin-sdk.json

# Expose ports for HTTP and HTTPS
EXPOSE 80
EXPOSE 443

# Set the entry point for the container
ENTRYPOINT ["dotnet", "Api.dll"]
