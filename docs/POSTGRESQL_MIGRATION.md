# 🐘 Guía de Migración a PostgreSQL

## 📋 Índice
1. [Requisitos Previos](#requisitos-previos)
2. [Instalación de PostgreSQL](#instalación-de-postgresql)
3. [Configuración de la Base de Datos](#configuración-de-la-base-de-datos)
4. [Configuración de la Aplicación](#configuración-de-la-aplicación)
5. [Migración de Datos](#migración-de-datos)
6. [Verificación](#verificación)
7. [Troubleshooting](#troubleshooting)
8. [Optimizaciones para Producción](#optimizaciones-para-producción)

---

## 🎯 Requisitos Previos

### Software Necesario
- ✅ .NET 8.0 SDK
- ✅ PostgreSQL 12+ (recomendado: 16+)
- ✅ pgAdmin 4 (opcional, para administración visual)
- ✅ Cliente psql (incluido con PostgreSQL)

### Paquetes NuGet (Ya Incluidos)
```xml
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.11" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.11" />
```

---

## 🐘 Instalación de PostgreSQL

### Windows

#### Opción 1: Instalador Oficial
```powershell
# Descargar desde https://www.postgresql.org/download/windows/
# Ejecutar el instalador y seguir los pasos

# Durante la instalación:
# - Puerto: 5432 (default)
# - Usuario: postgres
# - Contraseña: [tu_contraseña_segura]
# - Locale: Spanish, Spain
```

#### Opción 2: Chocolatey
```powershell
# Instalar Chocolatey si no lo tienes
Set-ExecutionPolicy Bypass -Scope Process -Force
[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072
iex ((New-Object System.Net.WebClient).DownloadString('https://chocolatey.org/install.ps1'))

# Instalar PostgreSQL
choco install postgresql16 -y

# Reiniciar terminal
```

#### Opción 3: Docker (Más Rápido)
```powershell
# Crear contenedor PostgreSQL
docker run -d `
  --name postgres-excursionistas `
  -e POSTGRES_PASSWORD=LzAD4401000401 `
  -e POSTGRES_DB=excursionistas_db `
  -e POSTGRES_USER=excursionistas_user `
  -p 5432:5432 `
  -v pgdata:/var/lib/postgresql/data `
  postgres:16-alpine

# Verificar que está corriendo
docker ps

# Logs
docker logs postgres-excursionistas

# Conectar con psql
docker exec -it postgres-excursionistas psql -U excursionistas_user -d excursionistas_db
```

### Linux (Ubuntu/Debian)
```bash
# Actualizar repositorios
sudo apt update

# Instalar PostgreSQL
sudo apt install postgresql postgresql-contrib -y

# Iniciar servicio
sudo systemctl start postgresql
sudo systemctl enable postgresql

# Verificar instalación
sudo systemctl status postgresql
```

### macOS
```bash
# Con Homebrew
brew install postgresql@16

# Iniciar servicio
brew services start postgresql@16

# Verificar
psql --version
```

### Verificar Instalación
```powershell
# Windows (PowerShell)
$Env:Path += ";C:\Program Files\PostgreSQL\16\bin"
psql --version
# Resultado esperado: psql (PostgreSQL) 16.x

# Verificar servicio está corriendo
Get-Service -Name postgresql*
# Estado debe ser "Running"
```

---

## 🔧 Configuración de la Base de Datos

### 1. Conectar a PostgreSQL

#### Windows (PowerShell)
```powershell
# Conectar como superusuario postgres
psql -U postgres

# O especificar host
psql -h localhost -p 5432 -U postgres
```

#### Desde pgAdmin 4
1. Abrir pgAdmin 4
2. Crear nuevo servidor
3. Configurar:
   - Name: Excursionistas Local
   - Host: localhost
   - Port: 5432
   - Username: postgres
   - Password: [tu_contraseña]

### 2. Crear Base de Datos y Usuario

```sql
-- Conectado como postgres, ejecutar:

-- 1. Crear usuario para la aplicación
CREATE USER excursionistas_user WITH PASSWORD 'LzAD4401000401';

-- 2. Crear base de datos
CREATE DATABASE excursionistas_db
    WITH 
    OWNER = excursionistas_user
    ENCODING = 'UTF8'
    LC_COLLATE = 'Spanish_Spain.1252'  -- Windows
    -- LC_COLLATE = 'es_ES.UTF-8'      -- Linux/macOS
    LC_CTYPE = 'Spanish_Spain.1252'     -- Windows
    -- LC_CTYPE = 'es_ES.UTF-8'        -- Linux/macOS
    TABLESPACE = pg_default
    CONNECTION LIMIT = -1;

-- 3. Otorgar privilegios
GRANT ALL PRIVILEGES ON DATABASE excursionistas_db TO excursionistas_user;

-- 4. Conectar a la nueva base de datos
\c excursionistas_db

-- 5. Otorgar privilegios en el schema public
GRANT ALL ON SCHEMA public TO excursionistas_user;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO excursionistas_user;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO excursionistas_user;

-- 6. Configurar permisos futuros
ALTER DEFAULT PRIVILEGES IN SCHEMA public
GRANT ALL ON TABLES TO excursionistas_user;

ALTER DEFAULT PRIVILEGES IN SCHEMA public
GRANT ALL ON SEQUENCES TO excursionistas_user;

-- 7. Verificar creación
\l                          -- Listar bases de datos
\du                         -- Listar usuarios
```

### 3. Verificar Conexión

```powershell
# Conectar con el nuevo usuario
psql -h localhost -p 5432 -U excursionistas_user -d excursionistas_db

# Dentro de psql:
SELECT version();            -- Ver versión de PostgreSQL
\conninfo                    -- Ver información de conexión
\dt                          -- Listar tablas (vacío por ahora)
```

---

## ⚙️ Configuración de la Aplicación

### 1. Actualizar archivo .env

#### Opción A: Editar .env directamente
```powershell
# Abrir .env con notepad
notepad .env
```

Cambiar las siguientes líneas:
```bash
# Antes:
DATABASE_PROVIDER=Sqlite
SQLITE_CONNECTION_STRING=Data Source=excursionistas.db

# Después:
DATABASE_PROVIDER=PostgreSql

# Descomentar y configurar PostgreSQL
POSTGRES_HOST=localhost
POSTGRES_PORT=5432
POSTGRES_DATABASE=excursionistas_db
POSTGRES_USER=excursionistas_user
POSTGRES_PASSWORD=LzAD4401000401
```

#### Opción B: Variables de Entorno del Sistema (Producción)
```powershell
# Windows PowerShell (Administrador)
[System.Environment]::SetEnvironmentVariable('DATABASE_PROVIDER', 'PostgreSql', 'Machine')
[System.Environment]::SetEnvironmentVariable('POSTGRES_HOST', 'localhost', 'Machine')
[System.Environment]::SetEnvironmentVariable('POSTGRES_PORT', '5432', 'Machine')
[System.Environment]::SetEnvironmentVariable('POSTGRES_DATABASE', 'excursionistas_db', 'Machine')
[System.Environment]::SetEnvironmentVariable('POSTGRES_USER', 'excursionistas_user', 'Machine')
[System.Environment]::SetEnvironmentVariable('POSTGRES_PASSWORD', 'LzAD4401000401', 'Machine')

# Reiniciar terminal para aplicar cambios
```

### 2. Verificar Configuración

```powershell
# Ver configuración actual
Get-Content .env | Select-String "DATABASE_PROVIDER|POSTGRES_"

# Resultado esperado:
# DATABASE_PROVIDER=PostgreSql
# POSTGRES_HOST=localhost
# POSTGRES_PORT=5432
# POSTGRES_DATABASE=excursionistas_db
# POSTGRES_USER=excursionistas_user
# POSTGRES_PASSWORD=LzAD4401000401
```

### 3. Limpiar Artefactos de SQLite (Opcional)

```powershell
# Eliminar base de datos SQLite anterior
Remove-Item -Path "excursionistas.db*" -Force -ErrorAction SilentlyContinue
Remove-Item -Path "data/excursionistas.db*" -Force -ErrorAction SilentlyContinue

# Limpiar compilación
dotnet clean
Remove-Item -Path "src/*/bin","src/*/obj","tests/*/bin","tests/*/obj" -Recurse -Force
```

---

## 🔄 Migración de Datos

### Paso 1: Generar Migraciones para PostgreSQL

```powershell
# Navegar al directorio del proyecto
cd C:\Users\Acer\Desktop\C#\Excursionistas

# Las migraciones ya existen, pero regenerarlas para PostgreSQL:
# (Opcional) Eliminar migraciones antiguas si quieres empezar limpio
# Remove-Item -Path "src/Excursionistas.Infrastructure/Data/Migrations/*" -Recurse -Force

# Crear nueva migración (si es necesario)
dotnet ef migrations add InitialPostgreSQL `
  --project src/Excursionistas.Infrastructure `
  --startup-project src/Excursionistas.API `
  --context ExcursionistasDbContext `
  --output-dir Data/Migrations

# Ver la migración generada
Get-Content "src/Excursionistas.Infrastructure/Data/Migrations/*_InitialPostgreSQL.cs" | Select-Object -First 50
```

### Paso 2: Aplicar Migraciones

#### Opción A: Automáticamente al Iniciar la App (Recomendado)
La aplicación ya aplica migraciones automáticamente en `Program.cs`:
```csharp
// Líneas 64-85 en Program.cs
await dbContext.Database.MigrateAsync();
```

Simplemente ejecutar:
```powershell
dotnet run --project src/Excursionistas.API
```

La aplicación:
1. ✅ Se conecta a PostgreSQL
2. ✅ Crea las tablas si no existen
3. ✅ Aplica migraciones pendientes
4. ✅ Ejecuta seeding de datos si `ENABLE_DATA_SEEDING=true`

#### Opción B: Manualmente con EF Core CLI
```powershell
# Aplicar migraciones manualmente
dotnet ef database update `
  --project src/Excursionistas.Infrastructure `
  --startup-project src/Excursionistas.API `
  --context ExcursionistasDbContext

# Verificar tablas creadas
psql -h localhost -U excursionistas_user -d excursionistas_db -c "\dt"

# Resultado esperado:
#  Schema |        Name        | Type  |       Owner
# --------+--------------------+-------+--------------------
#  public | Configurations     | table | excursionistas_user
#  public | Elements           | table | excursionistas_user
#  public | __EFMigrationsHistory | table | excursionistas_user
```

### Paso 3: Verificar Esquema

```sql
-- Conectar a PostgreSQL
psql -h localhost -U excursionistas_user -d excursionistas_db

-- Ver estructura de tabla Elements
\d "Elements"

-- Resultado esperado:
--                       Table "public.Elements"
--      Column       |           Type           | Nullable | Default
-- ------------------+--------------------------+----------+---------
--  Id               | integer                  | not null | nextval
--  Name             | character varying(200)   | not null |
--  Weight           | double precision         | not null |
--  Calories         | double precision         | not null |
--  CreatedAt        | timestamp with time zone | not null |
--  UpdatedAt        | timestamp with time zone |          |
--  IsActive         | boolean                  | not null |
-- Indexes:
--     "PK_Elements" PRIMARY KEY, btree ("Id")
--     "IX_Elements_Name" UNIQUE, btree ("Name")

-- Ver datos (debe estar vacío si no hay seeding)
SELECT COUNT(*) FROM "Elements";
```

### Paso 4: Migrar Datos desde SQLite (Opcional)

Si ya tienes datos en SQLite que quieres migrar:

```powershell
# 1. Exportar datos de SQLite a JSON
$sqliteDb = "excursionistas.db"
$elements = sqlite3 $sqliteDb "SELECT * FROM Elements" -json

# 2. Guardar en archivo
$elements | Out-File -FilePath "elements_export.json" -Encoding UTF8

# 3. Importar manualmente con script C# o copiar/pegar en PostgreSQL
# O usar el seeding automático que ya carga 20 elementos
```

### Paso 5: Ejecutar Seeding

```powershell
# Asegurarse que esté habilitado en .env
# ENABLE_DATA_SEEDING=true

# Ejecutar aplicación
dotnet run --project src/Excursionistas.API

# Logs esperados:
# info: Program[0] ✅ Migraciones aplicadas exitosamente
# info: Program[0] ✅ Datos iniciales cargados exitosamente
# info: Program[0] 🚀 Excursionistas API iniciada exitosamente
```

---

## ✅ Verificación

### 1. Verificar Conexión de la Aplicación

```powershell
# Ejecutar aplicación
$env:ASPNETCORE_ENVIRONMENT="Development"
dotnet run --project src/Excursionistas.API

# Ver logs para confirmar conexión PostgreSQL
# Esperado: "Now listening on: http://localhost:5002"
```

### 2. Verificar Datos en PostgreSQL

```sql
-- Conectar a psql
psql -h localhost -U excursionistas_user -d excursionistas_db

-- Ver cantidad de elementos
SELECT COUNT(*) FROM "Elements";
-- Esperado: 20

-- Ver primeros 5 elementos
SELECT "Id", "Name", "Weight", "Calories", "CalorieEfficiency" 
FROM "Elements" 
ORDER BY "CalorieEfficiency" DESC 
LIMIT 5;

-- Verificar configuraciones
SELECT * FROM "Configurations";
```

### 3. Probar API con PostgreSQL

```powershell
# GET todos los elementos
Invoke-RestMethod -Uri "http://localhost:5002/api/elements" | 
    Select-Object -First 3 | 
    Format-Table id, name, weight, calories

# POST crear nuevo elemento
$newElement = @{
    name = "Gel Energético PostgreSQL"
    weight = 0.05
    calories = 120
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5002/api/elements" `
    -Method Post `
    -Body $newElement `
    -ContentType "application/json"

# Verificar en PostgreSQL
psql -h localhost -U excursionistas_user -d excursionistas_db `
    -c "SELECT * FROM \"Elements\" WHERE \"Name\" LIKE '%PostgreSQL%';"
```

### 4. Ejecutar Tests

```powershell
# Los tests usan InMemory database, no PostgreSQL
dotnet test

# Resultado esperado: 24/24 tests ✅
```

---

## 🐛 Troubleshooting

### Error: "Could not connect to server"

**Síntoma:**
```
Npgsql.NpgsqlException: Failed to connect to localhost:5432
```

**Soluciones:**
```powershell
# 1. Verificar que PostgreSQL está corriendo
Get-Service -Name postgresql*
# Si está "Stopped", iniciar:
Start-Service postgresql-x64-16

# 2. Verificar puerto
netstat -an | findstr "5432"
# Debe mostrar: TCP 0.0.0.0:5432 LISTENING

# 3. Verificar firewall
Test-NetConnection -ComputerName localhost -Port 5432

# 4. Verificar pg_hba.conf permite conexiones locales
# Ubicación: C:\Program Files\PostgreSQL\16\data\pg_hba.conf
# Debe contener: host all all 127.0.0.1/32 md5
```

### Error: "Password authentication failed"

**Síntoma:**
```
Npgsql.PostgresException: 28P01: password authentication failed
```

**Soluciones:**
```powershell
# 1. Verificar contraseña en .env
Get-Content .env | Select-String "POSTGRES_PASSWORD"

# 2. Resetear contraseña en PostgreSQL
psql -U postgres
ALTER USER excursionistas_user WITH PASSWORD 'NuevaContraseña';
\q

# 3. Actualizar .env con nueva contraseña
```

### Error: "Database does not exist"

**Síntoma:**
```
Npgsql.PostgresException: 3D000: database "excursionistas_db" does not exist
```

**Soluciones:**
```powershell
# Crear base de datos manualmente
psql -U postgres -c "CREATE DATABASE excursionistas_db OWNER excursionistas_user;"

# O ejecutar el script completo de configuración
psql -U postgres -f setup_database.sql
```

### Error: "Permission denied for schema public"

**Síntoma:**
```
Npgsql.PostgresException: 42501: permission denied for schema public
```

**Soluciones:**
```sql
-- Conectar como postgres
psql -U postgres -d excursionistas_db

-- Otorgar permisos
GRANT ALL ON SCHEMA public TO excursionistas_user;
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO excursionistas_user;
GRANT ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA public TO excursionistas_user;
```

### Migración Falla: "Column already exists"

**Síntoma:**
```
Npgsql.PostgresException: 42701: column "Id" of relation "Elements" already exists
```

**Soluciones:**
```powershell
# 1. Limpiar base de datos
psql -U postgres -c "DROP DATABASE excursionistas_db;"
psql -U postgres -c "CREATE DATABASE excursionistas_db OWNER excursionistas_user;"

# 2. Volver a ejecutar migraciones
dotnet ef database update --project src/Excursionistas.Infrastructure --startup-project src/Excursionistas.API
```

---

## 🚀 Optimizaciones para Producción

### 1. Connection Pooling (Ya Configurado)

El connection string ya incluye pooling por defecto:
```csharp
// En ServiceCollectionExtensions.cs
options.UseNpgsql(connectionString, npgsqlOptions =>
{
    npgsqlOptions.EnableRetryOnFailure(
        maxRetryCount: 3,
        maxRetryDelay: TimeSpan.FromSeconds(5),
        errorCodesToAdd: null);
});
```

### 2. Índices Adicionales

```sql
-- Crear índices para mejorar rendimiento
CREATE INDEX IF NOT EXISTS "IX_Elements_IsActive" 
ON "Elements" ("IsActive") 
WHERE "IsActive" = true;

CREATE INDEX IF NOT EXISTS "IX_Elements_CalorieEfficiency" 
ON "Elements" ((calories / weight)) 
WHERE calories > 0 AND weight > 0;

CREATE INDEX IF NOT EXISTS "IX_Elements_CreatedAt" 
ON "Elements" ("CreatedAt" DESC);
```

### 3. Configurar PostgreSQL para Producción

```sql
-- Editar postgresql.conf
-- Ubicación: C:\Program Files\PostgreSQL\16\data\postgresql.conf

-- Ajustar memoria
shared_buffers = 256MB              # 25% de RAM
effective_cache_size = 1GB          # 50-75% de RAM
work_mem = 16MB                     # Por query
maintenance_work_mem = 128MB        # Para VACUUM, CREATE INDEX

-- Ajustar connections
max_connections = 100               # Default, ajustar según necesidad

-- Logging
log_statement = 'all'               # Solo en desarrollo
log_duration = on
log_line_prefix = '%t [%p]: [%l-1] user=%u,db=%d,app=%a,client=%h '

-- Reiniciar PostgreSQL
Restart-Service postgresql-x64-16
```

### 4. Backups Automatizados

```powershell
# Script de backup (backup_postgres.ps1)
$date = Get-Date -Format "yyyyMMdd_HHmmss"
$backupFile = "backup_excursionistas_$date.sql"

$env:PGPASSWORD = "LzAD4401000401"
pg_dump -h localhost -U excursionistas_user -d excursionistas_db -F c -f $backupFile

Write-Host "Backup creado: $backupFile"

# Configurar tarea programada (Task Scheduler)
# Ejecutar diariamente a las 2 AM
```

### 5. Monitoreo

```sql
-- Ver conexiones activas
SELECT 
    pid, 
    usename, 
    application_name, 
    client_addr, 
    state, 
    query_start, 
    query 
FROM pg_stat_activity 
WHERE datname = 'excursionistas_db';

-- Ver tamaño de base de datos
SELECT 
    pg_size_pretty(pg_database_size('excursionistas_db')) AS size;

-- Ver queries lentas
SELECT 
    mean_exec_time, 
    calls, 
    query 
FROM pg_stat_statements 
ORDER BY mean_exec_time DESC 
LIMIT 10;
```

---

## 📊 Comparación: SQLite vs PostgreSQL

| Aspecto | SQLite | PostgreSQL |
|---------|--------|------------|
| **Tipo** | Archivo embebido | Servidor cliente/servidor |
| **Concurrencia** | Limitada (1 escritor) | Alta (múltiples escritores) |
| **Tamaño máximo** | 281 TB (teórico), ~1GB práctico | Ilimitado (práctico: varios TB) |
| **Transacciones** | ACID | ACID + MVCC |
| **Usuarios concurrentes** | <10 | 100+ fácilmente |
| **Índices** | B-tree | B-tree, GiST, SP-GiST, GIN, BRIN |
| **Full-text search** | Limitado | Avanzado |
| **Replicación** | No nativa | Master-slave, streaming |
| **JSON** | Básico | JSONB con índices |
| **Triggers** | Básico | Avanzado con PL/pgSQL |
| **Ventanas** | Sí | Sí (más funciones) |
| **CTEs** | Sí | Sí + recursivos |
| **Particionamiento** | No | Sí (declarativo) |
| **Backup en caliente** | No | Sí |
| **Ideal para** | Desarrollo, apps móviles, <100GB | Producción, alta concurrencia, >1GB |

---

## ✅ Checklist de Migración

- [ ] PostgreSQL 12+ instalado
- [ ] Base de datos `excursionistas_db` creada
- [ ] Usuario `excursionistas_user` creado con permisos
- [ ] Archivo `.env` actualizado con `DATABASE_PROVIDER=PostgreSql`
- [ ] Variables de PostgreSQL configuradas (`POSTGRES_HOST`, etc.)
- [ ] Aplicación compila sin errores
- [ ] Migraciones aplicadas exitosamente
- [ ] Datos seedeados (20 elementos)
- [ ] API responde correctamente (`GET /api/elements`)
- [ ] Optimización funciona (`POST /api/optimization/calculate`)
- [ ] Tests pasan (24/24)
- [ ] Backup configurado
- [ ] Índices adicionales creados
- [ ] Monitoreo configurado

---

## 🎓 Recursos Adicionales

- [PostgreSQL Documentation](https://www.postgresql.org/docs/)
- [Npgsql Entity Framework Core Provider](https://www.npgsql.org/efcore/)
- [PostgreSQL Performance Tuning](https://wiki.postgresql.org/wiki/Performance_Optimization)
- [pgAdmin 4 Documentation](https://www.pgadmin.org/docs/)

---

**Guía creada:** 8 de Marzo, 2026  
**Versión Sistema:** 1.0.0  
**PostgreSQL Soportado:** 12, 13, 14, 15, 16+  
**Autor:** Anderson Lozano
