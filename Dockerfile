# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/Excursionistas.API/Excursionistas.API.csproj", "src/Excursionistas.API/"]
COPY ["src/Excursionistas.Application/Excursionistas.Application.csproj", "src/Excursionistas.Application/"]
COPY ["src/Excursionistas.Domain/Excursionistas.Domain.csproj", "src/Excursionistas.Domain/"]
COPY ["src/Excursionistas.Infrastructure/Excursionistas.Infrastructure.csproj", "src/Excursionistas.Infrastructure/"]

RUN dotnet restore "src/Excursionistas.API/Excursionistas.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/src/Excursionistas.API"
RUN dotnet build "Excursionistas.API.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "Excursionistas.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 5000

# Copy published files
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Excursionistas.API.dll"]
