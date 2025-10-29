# Use ASP.NET 9.0 runtime on Linux as base image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
EXPOSE 8081

# Use .NET 9.0 SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project file and restore dependencies
COPY ["CarsDealersManagement.Microservice/CarsDealersManagement.Microservice.csproj", "CarsDealersManagement.Microservice/"]
RUN dotnet restore "CarsDealersManagement.Microservice/CarsDealersManagement.Microservice.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/CarsDealersManagement.Microservice"
RUN dotnet build "CarsDealersManagement.Microservice.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the project
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/CarsDealersManagement.Microservice"
RUN dotnet publish "CarsDealersManagement.Microservice.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set the entry point
ENTRYPOINT ["dotnet", "CarsDealersManagement.Microservice.dll"]
