# 🚀 Guía de Deployment - Excursionistas API

## 📋 Índice
1. [Overview](#overview)
2. [Ambientes](#ambientes)
3. [Deployment Local](#deployment-local)
4. [Deployment con Docker](#deployment-con-docker)
5. [Deployment en Azure](#deployment-en-azure)
6. [Deployment en AWS](#deployment-en-aws)
7. [CI/CD con GitHub Actions](#cicd-con-github-actions)
8. [Monitoreo y Logging](#monitoreo-y-logging)
9. [Checklist de Deployment](#checklist-de-deployment)
10. [Troubleshooting](#troubleshooting)

---

## 🎯 Overview

Esta guía cubre todos los métodos de deployment para **Excursionistas API**, desde desarrollo local hasta producción en la nube.

### Estrategias de Deployment

| Estrategia | Cuándo Usar | Complejidad | Costo |
|------------|-------------|-------------|-------|
| **Local Development** | Desarrollo y pruebas | Baja | Gratis |
| **Docker Local** | Testing de containerización | Media | Gratis |
| **Docker Compose** | Multi-container local | Media | Gratis |
| **Azure App Service** | Producción pequeña/mediana | Media | $$ |
| **Azure Container Apps** | Producción escalable | Alta | $$$ |
| **AWS ECS/Fargate** | Producción AWS | Alta | $$$ |
| **Kubernetes (AKS/EKS)** | Producción enterprise | Muy Alta | $$$$ |

---

## 🌍 Ambientes

### 1. Development (SQLite)
- **Base de datos**: SQLite (archivo local)
- **Propósito**: Desarrollo y debugging local
- **Seeding**: Habilitado por defecto
- **Logs**: Nivel Debug
- **HTTPS**: Certificado dev

### 2. Staging (PostgreSQL)
- **Base de datos**: PostgreSQL (Docker o Azure Database)
- **Propósito**: Testing pre-producción
- **Seeding**: Opcional
- **Logs**: Nivel Information
- **HTTPS**: Certificado válido

### 3. Production (PostgreSQL)
- **Base de datos**: PostgreSQL (Azure/AWS managed)
- **Propósito**: Usuarios finales
- **Seeding**: Deshabilitado
- **Logs**: Nivel Warning
- **HTTPS**: Certificado válido + HSTS

---

## 💻 Deployment Local

### Requisitos
- .NET 8.0 SDK
- SQLite (incluido) o PostgreSQL 16+
- Visual Studio 2022 / VS Code / Rider

### Paso 1: Preparar Entorno

```powershell
# Clonar repositorio
git clone https://github.com/LozanoAndersonTheStain/prueba-excursionistas.git
cd prueba-excursionistas

# Restaurar paquetes
dotnet restore

# Verificar que compila
dotnet build
```

### Paso 2: Configurar .env

```bash
# Crear archivo .env (o copiar .env.example)
DATABASE_PROVIDER=Sqlite
SQLITE_CONNECTION_STRING=Data Source=excursionistas.db
ASPNETCORE_ENVIRONMENT=Development
API_PORT=5000
API_HTTPS_PORT=5001
ENABLE_DATA_SEEDING=true
CORS_ALLOWED_ORIGINS=http://localhost:3000,http://localhost:4200
```

### Paso 3: Ejecutar

```powershell
# Ejecutar API
dotnet run --project src/Excursionistas.API

# O con hot reload
dotnet watch run --project src/Excursionistas.API

# La API estará disponible en:
# - http://localhost:5000
# - https://localhost:5001
# - Swagger UI: http://localhost:5000
```

### Paso 4: Verificar

```powershell
# Probar endpoint de salud (si existe)
Invoke-RestMethod -Uri "http://localhost:5000/health"

# Probar GET elementos
Invoke-RestMethod -Uri "http://localhost:5000/api/elements"

# Ver 3 primeros elementos
Invoke-RestMethod -Uri "http://localhost:5000/api/elements" | Select-Object -First 3
```

### Paso 5: Ejecutar Tests

```powershell
# Todos los tests
dotnet test

# Solo unit tests
dotnet test --filter "FullyQualifiedName~UnitTests"

# Solo integration tests
dotnet test --filter "FullyQualifiedName~IntegrationTests"

# Con cobertura
dotnet test --collect:"XPlat Code Coverage"
```

---

## 🐳 Deployment con Docker

### Requisitos
- Docker Desktop 20+
- Docker Compose 3.8+

### Método 1: Docker Simple (SQLite)

#### Paso 1: Build

```powershell
# Build imagen
docker build -t excursionistas:latest .

# Verificar imagen creada
docker images | Select-String "excursionistas"

# Resultado esperado:
# excursionistas   latest   abc123def456   2 minutes ago   393MB
```

#### Paso 2: Run

```powershell
# Ejecutar contenedor
docker run -d `
  --name excursionistas-api `
  -p 5000:8080 `
  -e ASPNETCORE_ENVIRONMENT=Development `
  -e DATABASE_PROVIDER=Sqlite `
  -e SQLITE_CONNECTION_STRING="Data Source=/app/data/excursionistas.db" `
  -v ${PWD}/data:/app/data `
  excursionistas:latest

# Ver logs
docker logs excursionistas-api

# Seguir logs en tiempo real
docker logs -f excursionistas-api
```

#### Paso 3: Verificar

```powershell
# Probar API
Invoke-RestMethod -Uri "http://localhost:5000/api/elements"

# Ver estado del contenedor
docker ps | Select-String "excursionistas"

# Inspeccionar contenedor
docker inspect excursionistas-api
```

#### Paso 4: Detener y Limpiar

```powershell
# Detener contenedor
docker stop excursionistas-api

# Eliminar contenedor
docker rm excursionistas-api

# Eliminar imagen (opcional)
docker rmi excursionistas:latest
```

### Método 2: Docker Compose (PostgreSQL)

#### Paso 1: Preparar docker-compose.yml

Crear archivo `docker-compose.production.yml`:

```yaml
version: '3.8'

services:
  # PostgreSQL Database
  postgres:
    image: postgres:16-alpine
    container_name: excursionistas-postgres
    restart: unless-stopped
    environment:
      POSTGRES_USER: excursionistas_user
      POSTGRES_PASSWORD: ${POSTGRES_PASSWORD:-LzAD4401000401}
      POSTGRES_DB: excursionistas_db
      PGDATA: /var/lib/postgresql/data/pgdata
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U excursionistas_user -d excursionistas_db"]
      interval: 10s
      timeout: 5s
      retries: 5
    networks:
      - excursionistas-network

  # pgAdmin (opcional)
  pgadmin:
    image: dpage/pgadmin4:latest
    container_name: excursionistas-pgadmin
    restart: unless-stopped
    environment:
      PGADMIN_DEFAULT_EMAIL: admin@excursionistas.com
      PGADMIN_DEFAULT_PASSWORD: admin123
      PGADMIN_CONFIG_SERVER_MODE: 'False'
    ports:
      - "5050:80"
    depends_on:
      - postgres
    networks:
      - excursionistas-network

  # API Application
  api:
    build:
      context: .
      dockerfile: Dockerfile
    image: excursionistas:latest
    container_name: excursionistas-api
    restart: unless-stopped
    environment:
      ASPNETCORE_ENVIRONMENT: Production
      ASPNETCORE_URLS: http://+:8080
      DATABASE_PROVIDER: PostgreSql
      POSTGRES_HOST: postgres
      POSTGRES_PORT: 5432
      POSTGRES_DATABASE: excursionistas_db
      POSTGRES_USER: excursionistas_user
      POSTGRES_PASSWORD: ${POSTGRES_PASSWORD:-LzAD4401000401}
      ENABLE_DATA_SEEDING: "true"
      CORS_ALLOWED_ORIGINS: ${CORS_ORIGINS:-*}
    ports:
      - "5000:8080"
    depends_on:
      postgres:
        condition: service_healthy
    networks:
      - excursionistas-network
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:8080/health"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 40s

volumes:
  postgres_data:
    driver: local

networks:
  excursionistas-network:
    driver: bridge
```

#### Paso 2: Crear archivo .env.production

```bash
# PostgreSQL
POSTGRES_PASSWORD=TuPasswordSeguroProduccion123!

# CORS
CORS_ORIGINS=https://tudominio.com,https://www.tudominio.com

# Environment
ASPNETCORE_ENVIRONMENT=Production
```

#### Paso 3: Ejecutar Stack Completo

```powershell
# Levantar todos los servicios
docker-compose -f docker-compose.production.yml --env-file .env.production up -d

# Ver logs de todos los servicios
docker-compose -f docker-compose.production.yml logs -f

# Ver logs solo de la API
docker-compose -f docker-compose.production.yml logs -f api

# Ver logs solo de PostgreSQL
docker-compose -f docker-compose.production.yml logs -f postgres
```

#### Paso 4: Verificar Servicios

```powershell
# Ver estado de servicios
docker-compose -f docker-compose.production.yml ps

# Resultado esperado:
# NAME                    STATUS              PORTS
# excursionistas-api      Up 2 minutes        0.0.0.0:5000->8080/tcp
# excursionistas-postgres Up 2 minutes        0.0.0.0:5432->5432/tcp
# excursionistas-pgadmin  Up 2 minutes        0.0.0.0:5050->80/tcp

# Probar API
Invoke-RestMethod -Uri "http://localhost:5000/api/elements"

# Acceder a pgAdmin: http://localhost:5050
# Email: admin@excursionistas.com
# Password: admin123
```

#### Paso 5: Gestionar Stack

```powershell
# Detener servicios (sin eliminar datos)
docker-compose -f docker-compose.production.yml stop

# Iniciar servicios detenidos
docker-compose -f docker-compose.production.yml start

# Reiniciar servicios
docker-compose -f docker-compose.production.yml restart

# Detener y eliminar contenedores (mantiene volúmenes)
docker-compose -f docker-compose.production.yml down

# Detener y eliminar TODO (incluye volúmenes)
docker-compose -f docker-compose.production.yml down -v

# Reconstruir imagen y reiniciar
docker-compose -f docker-compose.production.yml up -d --build
```

---

## ☁️ Deployment en Azure

### Opción 1: Azure App Service (PaaS)

#### Paso 1: Crear Recursos en Azure

```powershell
# Instalar Azure CLI
# https://learn.microsoft.com/cli/azure/install-azure-cli

# Login
az login

# Configurar variables
$RESOURCE_GROUP="rg-excursionistas-prod"
$LOCATION="eastus"
$APP_SERVICE_PLAN="plan-excursionistas-prod"
$WEB_APP_NAME="excursionistas-api-$(Get-Random)"
$POSTGRES_SERVER="psql-excursionistas-prod"
$POSTGRES_ADMIN_USER="pgadmin"
$POSTGRES_ADMIN_PASSWORD="SuperSecurePassword123!"
$POSTGRES_DB="excursionistas_db"

# Crear resource group
az group create --name $RESOURCE_GROUP --location $LOCATION

# Crear App Service Plan (Linux, B1 tier)
az appservice plan create `
  --name $APP_SERVICE_PLAN `
  --resource-group $RESOURCE_GROUP `
  --sku B1 `
  --is-linux

# Crear Web App
az webapp create `
  --resource-group $RESOURCE_GROUP `
  --plan $APP_SERVICE_PLAN `
  --name $WEB_APP_NAME `
  --runtime "DOTNETCORE:8.0"

# Crear PostgreSQL Flexible Server
az postgres flexible-server create `
  --resource-group $RESOURCE_GROUP `
  --name $POSTGRES_SERVER `
  --location $LOCATION `
  --admin-user $POSTGRES_ADMIN_USER `
  --admin-password $POSTGRES_ADMIN_PASSWORD `
  --sku-name Standard_B1ms `
  --tier Burstable `
  --storage-size 32 `
  --version 16

# Crear database
az postgres flexible-server db create `
  --resource-group $RESOURCE_GROUP `
  --server-name $POSTGRES_SERVER `
  --database-name $POSTGRES_DB

# Permitir acceso desde Azure services
az postgres flexible-server firewall-rule create `
  --resource-group $RESOURCE_GROUP `
  --name $POSTGRES_SERVER `
  --rule-name AllowAzureServices `
  --start-ip-address 0.0.0.0 `
  --end-ip-address 0.0.0.0
```

#### Paso 2: Configurar Variables de Entorno

```powershell
# Configurar connection string PostgreSQL
$CONNECTION_STRING="Host=$POSTGRES_SERVER.postgres.database.azure.com;Database=$POSTGRES_DB;Username=$POSTGRES_ADMIN_USER;Password=$POSTGRES_ADMIN_PASSWORD;SSL Mode=Require"

# Configurar app settings
az webapp config appsettings set `
  --resource-group $RESOURCE_GROUP `
  --name $WEB_APP_NAME `
  --settings `
    ASPNETCORE_ENVIRONMENT="Production" `
    DATABASE_PROVIDER="PostgreSql" `
    POSTGRES_CONNECTION_STRING="$CONNECTION_STRING" `
    ENABLE_DATA_SEEDING="true" `
    CORS_ALLOWED_ORIGINS="https://$WEB_APP_NAME.azurewebsites.net"

# Ver configuración
az webapp config appsettings list `
  --resource-group $RESOURCE_GROUP `
  --name $WEB_APP_NAME `
  --output table
```

#### Paso 3: Deploy desde Local

```powershell
# Publicar aplicación
dotnet publish src/Excursionistas.API/Excursionistas.API.csproj `
  -c Release `
  -o ./publish

# Crear ZIP para deployment
Compress-Archive -Path ./publish/* -DestinationPath ./deploy.zip -Force

# Deploy a Azure
az webapp deployment source config-zip `
  --resource-group $RESOURCE_GROUP `
  --name $WEB_APP_NAME `
  --src ./deploy.zip

# Ver logs de deployment
az webapp log tail --resource-group $RESOURCE_GROUP --name $WEB_APP_NAME
```

#### Paso 4: Verificar Deployment

```powershell
# Obtener URL
$APP_URL="https://$WEB_APP_NAME.azurewebsites.net"
Write-Host "API URL: $APP_URL"

# Probar endpoint
Invoke-RestMethod -Uri "$APP_URL/api/elements"

# Ver logs en Azure Portal
# https://portal.azure.com → App Service → Log stream
```

### Opción 2: Azure Container Apps (Serverless Containers)

```powershell
# Crear Container Apps environment
$CONTAINERAPPS_ENV="env-excursionistas-prod"

az containerapp env create `
  --name $CONTAINERAPPS_ENV `
  --resource-group $RESOURCE_GROUP `
  --location $LOCATION

# Crear Container App desde ACR
az containerapp create `
  --name excursionistas-api `
  --resource-group $RESOURCE_GROUP `
  --environment $CONTAINERAPPS_ENV `
  --image excursionistas:latest `
  --target-port 8080 `
  --ingress external `
  --env-vars `
    ASPNETCORE_ENVIRONMENT=Production `
    DATABASE_PROVIDER=PostgreSql `
    POSTGRES_CONNECTION_STRING="$CONNECTION_STRING"
```

---

## 🌩️ Deployment en AWS

### Opción 1: AWS Elastic Beanstalk

#### Paso 1: Instalar EB CLI

```powershell
# Instalar con pip
pip install awsebcli --upgrade --user

# Verificar instalación
eb --version
```

#### Paso 2: Inicializar Proyecto

```powershell
# Inicializar EB
eb init -p "64bit Amazon Linux 2 v2.7.0 running .NET 8" --region us-east-1

# Crear aplicación
eb create excursionistas-prod --instance-type t3.micro

# Configurar variables de entorno
eb setenv `
  ASPNETCORE_ENVIRONMENT=Production `
  DATABASE_PROVIDER=PostgreSql `
  POSTGRES_HOST=your-rds-endpoint.rds.amazonaws.com `
  POSTGRES_PORT=5432 `
  POSTGRES_DATABASE=excursionistas_db `
  POSTGRES_USER=postgres `
  POSTGRES_PASSWORD=YourSecurePassword

# Deploy
eb deploy

# Ver status
eb status

# Abrir en navegador
eb open
```

### Opción 2: AWS ECS Fargate

Ver documentación oficial de AWS ECS para deployment con contenedores.

---

## 🔄 CI/CD con GitHub Actions

### Archivo: `.github/workflows/deploy.yml`

```yaml
name: Build, Test & Deploy

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main ]

env:
  DOTNET_VERSION: '8.0.x'
  AZURE_WEBAPP_NAME: excursionistas-api-prod

jobs:
  # Job 1: Build and Test
  build-and-test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: ${{ env.DOTNET_VERSION }}
      
      - name: Restore dependencies
        run: dotnet restore
      
      - name: Build
        run: dotnet build --configuration Release --no-restore
      
      - name: Run Unit Tests
        run: dotnet test --no-build --verbosity normal --filter "FullyQualifiedName~UnitTests"
      
      - name: Run Integration Tests
        run: dotnet test --no-build --verbosity normal --filter "FullyQualifiedName~IntegrationTests"
      
      - name: Publish
        run: dotnet publish src/Excursionistas.API/Excursionistas.API.csproj -c Release -o ./publish
      
      - name: Upload artifact
        uses: actions/upload-artifact@v4
        with:
          name: webapp
          path: ./publish

  # Job 2: Deploy to Azure (only on main branch)
  deploy-azure:
    needs: build-and-test
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/main'
    steps:
      - name: Download artifact
        uses: actions/download-artifact@v4
        with:
          name: webapp
          path: ./publish
      
      - name: Deploy to Azure Web App
        uses: azure/webapps-deploy@v2
        with:
          app-name: ${{ env.AZURE_WEBAPP_NAME }}
          publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
          package: ./publish

  # Job 3: Build and Push Docker Image
  docker:
    needs: build-and-test
    runs-on: ubuntu-latest
    if: github.ref == 'refs/heads/main'
    steps:
      - uses: actions/checkout@v4
      
      - name: Log in to Docker Hub
        uses: docker/login-action@v3
        with:
          username: ${{ secrets.DOCKER_USERNAME }}
          password: ${{ secrets.DOCKER_PASSWORD }}
      
      - name: Build and push Docker image
        uses: docker/build-push-action@v5
        with:
          context: .
          push: true
          tags: |
            lozanoandersonthestain/excursionistas:latest
            lozanoandersonthestain/excursionistas:${{ github.sha }}
```

### Configurar Secrets en GitHub

1. Ir a GitHub → Settings → Secrets and variables → Actions
2. Agregar secrets:
   - `AZURE_WEBAPP_PUBLISH_PROFILE`: Descargar desde Azure Portal
   - `DOCKER_USERNAME`: Tu usuario de Docker Hub
   - `DOCKER_PASSWORD`: Tu token de Docker Hub

---

## 📊 Monitoreo y Logging

### Application Insights (Azure)

#### Paso 1: Agregar Paquete NuGet

```powershell
dotnet add src/Excursionistas.API package Microsoft.ApplicationInsights.AspNetCore
```

#### Paso 2: Configurar en Program.cs

```csharp
// En Program.cs
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
});
```

#### Paso 3: Agregar Variables de Entorno

```bash
APPLICATIONINSIGHTS_CONNECTION_STRING=InstrumentationKey=xxx;IngestionEndpoint=https://...
```

### Serilog (Logging Estructurado)

```powershell
# Agregar paquetes
dotnet add src/Excursionistas.API package Serilog.AspNetCore
dotnet add src/Excursionistas.API package Serilog.Sinks.Console
dotnet add src/Excursionistas.API package Serilog.Sinks.File
dotnet add src/Excursionistas.API package Serilog.Sinks.ApplicationInsights
```

### Health Checks

Ya implementado en `Program.cs`:
```csharp
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ExcursionistasDbContext>();

app.MapHealthChecks("/health");
```

Probar:
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/health"
```

---

## ✅ Checklist de Deployment

### Pre-Deployment

- [ ] Todos los tests pasan (24/24)
- [ ] Código revisado y aprobado
- [ ] Variables de entorno configuradas
- [ ] Connection strings actualizados
- [ ] Secrets almacenados de forma segura
- [ ] CORS configurado correctamente
- [ ] HTTPS habilitado
- [ ] Certificados SSL válidos

### Durante Deployment

- [ ] Backup de base de datos realizado
- [ ] Migraciones aplicadas exitosamente
- [ ] Health check responde OK
- [ ] API endpoints funcionan correctamente
- [ ] Swagger/OpenAPI accesible (Development)
- [ ] Logs configurados y funcionando
- [ ] Monitoreo activo

### Post-Deployment

- [ ] Pruebas de humo completadas
- [ ] Datos seedeados correctamente
- [ ] Performance aceptable
- [ ] Rollback plan preparado
- [ ] Documentación actualizada
- [ ] Equipo notificado

---

## 🐛 Troubleshooting

### Error: "Application failed to start"

**Síntomas:**
```
Application startup exception: ...
```

**Solución:**
```powershell
# Ver logs detallados
docker logs excursionistas-api

# O en Azure
az webapp log tail --resource-group $RESOURCE_GROUP --name $WEB_APP_NAME

# Verificar variables de entorno
docker exec -it excursionistas-api printenv | grep DATABASE
```

### Error: "Database connection failed"

**Solución:**
```powershell
# Probar conexión PostgreSQL
psql -h $POSTGRES_HOST -U $POSTGRES_USER -d $POSTGRES_DB

# Verificar firewall rules en Azure
az postgres flexible-server firewall-rule list `
  --resource-group $RESOURCE_GROUP `
  --name $POSTGRES_SERVER

# Probar desde contenedor
docker exec -it excursionistas-api /bin/bash
apt-get update && apt-get install -y postgresql-client
psql "Host=...;Database=...;Username=...;Password=..."
```

### Error: "502 Bad Gateway" en Azure

**Solución:**
```powershell
# Reiniciar Web App
az webapp restart --resource-group $RESOURCE_GROUP --name $WEB_APP_NAME

# Verificar logs
az webapp log tail --resource-group $RESOURCE_GROUP --name $WEB_APP_NAME

# Verificar configuración
az webapp config show --resource-group $RESOURCE_GROUP --name $WEB_APP_NAME
```

---

## 📚 Recursos Adicionales

- [Azure App Service Documentation](https://learn.microsoft.com/azure/app-service/)
- [Docker Documentation](https://docs.docker.com/)
- [GitHub Actions Documentation](https://docs.github.com/actions)
- [PostgreSQL on Azure](https://learn.microsoft.com/azure/postgresql/)
- [.NET Deployment Guide](https://learn.microsoft.com/dotnet/core/deploying/)

---

**Guía creada:** 8 de Marzo, 2026  
**Versión Sistema:** 1.0.0  
**Autor:** Anderson Lozano
