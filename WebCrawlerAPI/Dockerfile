# Use the official .NET SDK as a build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory in the container
WORKDIR /app

# Copy the project file and restore dependencies
COPY WebCrawlerAPI.csproj ./
RUN dotnet restore

# Copy the rest of the application source code and build it
COPY . ./
RUN dotnet publish -c Release -o out

# Use a smaller runtime image for the final build
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the published application from the build stage
COPY --from=build /app/out .

# Expose the port the app runs on
EXPOSE 80

# Start the application
ENTRYPOINT ["dotnet", "WebCrawlerAPI.dll"]
