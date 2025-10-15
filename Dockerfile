# Stage 1: Base SDK (for development)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS base
WORKDIR /src

# Install dotnet tools for hot reload
RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"

# Copy csproj and restore dependencies
COPY ["dotnet-docker-app.csproj", "./"]
RUN dotnet restore "dotnet-docker-app.csproj"

# Stage 2: Build
FROM base AS build
WORKDIR /src

# Copy everything else and build
COPY . .
RUN dotnet build "dotnet-docker-app.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "dotnet-docker-app.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Expose port
EXPOSE 8080
EXPOSE 8081

# Copy published files from publish stage
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Run the application
ENTRYPOINT ["dotnet", "dotnet-docker-app.dll"]
