# ----------------
# Build stage
# ----------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the entire solution
COPY . .

# Restore & publish the API project
RUN dotnet restore Shop.Api/Shop.Api.csproj
RUN dotnet publish Shop.Api/Shop.Api.csproj -c Release -o /app/publish

# ----------------
# Runtime stage
# ----------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

# Copy published files from build stage
COPY --from=build /app/publish .

# Expose the API port
EXPOSE 8080

# Start the API, connection string will be passed via environment variable
ENTRYPOINT ["dotnet", "Shop.Api.dll"]
