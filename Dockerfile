# Multi-stage Dockerfile for ShopSpark.API (ASP.NET Core 8 / .NET 8)
# Builds the solution, publishes the ShopSpark.API project, and runs the published DLL.

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and projects
COPY . .

# Restore only the ShopSpark.API project (faster incremental restore)
RUN dotnet restore "ShopSparkAPI/ShopSpark.API/ShopSpark.API.csproj"

# Publish the API project
RUN dotnet publish "ShopSparkAPI/ShopSpark.API/ShopSpark.API.csproj" -c Release -o /app/publish --no-restore

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Bind to port 5000 inside container (Render expects the web service to listen on a port)
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

# Copy published output from the build stage
COPY --from=build /app/publish .

# Entry point
ENTRYPOINT ["dotnet", "ShopSpark.API.dll"]
