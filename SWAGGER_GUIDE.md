# 📚 Guía de Swagger UI - Excursionistas API

## ✅ Configuración Completada

Se ha habilitado Swagger/OpenAPI para la API de Excursionistas con las siguientes características:

### 🎯 Acceso
- **URL Local (Development)**: http://localhost:5002
- **Puerto Docker**: http://localhost:5000 (sin Swagger, solo para producción)

### 📦 Componentes Configurados

1. **Swashbuckle.AspNetCore 6.5.0**
   - Compatible con .NET 8.0
   - Generación automática de documentación OpenAPI 3.0

2. **Documentación XML**
   - Comentarios del código convertidos automáticamente en documentación
   - Descripciones detalladas de endpoints, parámetros y respuestas
   - Ejemplos de uso incluidos

3. **Metadata Completa**
   - Título: "Excursionistas API"
   - Versión: v1
   - Descripción: API para optimización de selección de equipo de escalada
   - Contacto: GitHub Repository

---

## 🚀 Cómo Usar Swagger UI

### 1️⃣ Explorar Endpoints

Al abrir http://localhost:5002 verás una interfaz interactiva con todos los endpoints:

#### **Elements Controller** (CRUD de Elementos)
- `GET /api/elements` - Listar todos los elementos
- `GET /api/elements/{id}` - Obtener un elemento por ID
- `POST /api/elements` - Crear un nuevo elemento
- `PUT /api/elements/{id}` - Actualizar un elemento existente
- `DELETE /api/elements/{id}` - Eliminar (soft delete) un elemento

#### **Optimization Controller** (Algoritmo de Optimización)
- `POST /api/optimization/calculate` - Calcular combinación óptima

### 2️⃣ Probar Endpoints Interactivamente

Cada endpoint tiene un botón **"Try it out"**:

1. **Hacer clic en el endpoint** que quieres probar
2. **Hacer clic en "Try it out"**
3. **Editar los parámetros** (Swagger muestra los modelos con ejemplos)
4. **Hacer clic en "Execute"**
5. **Ver la respuesta** en tiempo real con:
   - Código de estado HTTP
   - Headers de respuesta
   - Body de respuesta (JSON)
   - Curl command equivalente

### 3️⃣ Ver Modelos (Schemas)

En la parte inferior de Swagger verás la sección **"Schemas"** con todos los DTOs:

- **CreateElementRequest**: Modelo para crear elementos
- **UpdateElementRequest**: Modelo para actualizar elementos
- **CalculateOptimizationRequest**: Modelo para solicitar optimización
- **ElementResponse**: Respuesta con datos de elemento
- **OptimizationResultResponse**: Respuesta con resultado de optimización
- **ErrorResponse**: Respuesta estandarizada de errores

Cada modelo muestra:
- ✅ Propiedades con tipos de dato
- ✅ Campos requeridos marcados con asterisco (*)
- ✅ Descripciones de cada campo
- ✅ Validaciones y restricciones

---

## 🎪 Ejemplos de Pruebas en Swagger

### Ejemplo 1: Listar Todos los Elementos

1. **Expandir**: `GET /api/elements`
2. **Hacer clic**: "Try it out"
3. **Hacer clic**: "Execute"
4. **Ver resultado**: Lista de 20 elementos con sus propiedades

**Respuesta esperada**: Código 200 con JSON array

### Ejemplo 2: Crear un Nuevo Elemento

1. **Expandir**: `POST /api/elements`
2. **Hacer clic**: "Try it out"
3. **Editar el JSON**:
```json
{
  "name": "Gel Energético",
  "weight": 0.05,
  "calories": 120
}
```
4. **Hacer clic**: "Execute"
5. **Ver resultado**: Elemento creado con código 201

**Validaciones automáticas**:
- El nombre no puede estar vacío
- El peso debe ser mayor a 0
- Las calorías deben ser >= 0

### Ejemplo 3: Calcular Optimización

1. **Expandir**: `POST /api/optimization/calculate`
2. **Hacer clic**: "Try it out"
3. **Editar el JSON**:
```json
{
  "minimumCalories": 1500,
  "maximumWeight": 5,
  "elementIds": []
}
```
4. **Hacer clic**: "Execute"
5. **Ver resultado**: Combinación óptima de elementos

**Respuesta esperada**:
```json
{
  "success": true,
  "message": "Solución encontrada mediante estrategia greedy: 4 elemento(s)",
  "selectedElements": [
    {
      "id": 5,
      "name": "Barra energética",
      "weight": 0.1,
      "calories": 300,
      "calorieEfficiency": 3000
    },
    // ... más elementos
  ],
  "totalWeight": 1.0,
  "totalCalories": 1750,
  "itemCount": 4,
  "averageEfficiency": 1750
}
```

### Ejemplo 4: Probar Validación (Error Esperado)

1. **Expandir**: `POST /api/elements`
2. **Hacer clic**: "Try it out"
3. **Editar el JSON** con datos inválidos:
```json
{
  "name": "",
  "weight": -1,
  "calories": -500
}
```
4. **Hacer clic**: "Execute"
5. **Ver resultado**: Código 400 Bad Request con mensajes de validación

---

## 📊 Códigos de Respuesta HTTP

Swagger muestra todos los posibles códigos de respuesta:

### Códigos de Éxito (2xx)
- **200 OK**: Operación exitosa (GET, POST optimization)
- **201 Created**: Recurso creado exitosamente (POST create)
- **204 No Content**: Operación exitosa sin contenido (DELETE)

### Códigos de Error del Cliente (4xx)
- **400 Bad Request**: Datos inválidos o validación fallida
- **404 Not Found**: Elemento no encontrado
- **409 Conflict**: Conflicto (ej: nombre duplicado)

### Códigos de Error del Servidor (5xx)
- **500 Internal Server Error**: Error no manejado

---

## 🔍 Características Avanzadas de Swagger

### 1. Autenticación
Actualmente la API no requiere autenticación, pero Swagger está preparado para agregar:
- Bearer Token
- API Keys
- OAuth2

### 2. Curl Command
Cada request ejecutado muestra el comando curl equivalente:
```bash
curl -X 'GET' \
  'http://localhost:5002/api/elements' \
  -H 'accept: application/json'
```

Puedes copiar y ejecutar estos comandos en tu terminal.

### 3. Request URL
Swagger muestra la URL completa usada:
```
http://localhost:5002/api/elements
```

### 4. Descarga de Especificación OpenAPI
Puedes descargar el archivo `swagger.json` completo desde:
```
http://localhost:5002/swagger/v1/swagger.json
```

Este archivo puede ser usado con:
- Postman (importar colección)
- Insomnia
- API Testing tools
- Code generators (AutoRest, OpenAPI Generator)

---

## 🎨 Personalización Futura

Posibles mejoras para Swagger:

1. **Temas Personalizados**: Cambiar colores y logo
2. **Ejemplos Múltiples**: Agregar varios ejemplos por endpoint
3. **Autenticación**: Integrar JWT Bearer tokens
4. **Agrupación**: Organizar endpoints por tags
5. **Versionado**: Soporte para múltiples versiones de API (v1, v2)
6. **Filters**: Agregar headers personalizados automáticamente

---

## 🐛 Resolución de Problemas

### Swagger no carga
```powershell
# Verificar que la API esté corriendo
Invoke-RestMethod -Uri "http://localhost:5002/api/elements"

# Si no responde, reiniciar
dotnet run --project src/Excursionistas.API
```

### XML Documentation no aparece
El archivo `Excursionistas.API.xml` debe generarse automáticamente en `bin/Debug/net8.0/`. 

Verificar en el `.csproj`:
```xml
<GenerateDocumentationFile>true</GenerateDocumentationFile>
```

### Error 404 en Swagger UI
Swagger solo está disponible en modo **Development**. Verificar:
```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
```

---

## 📝 Configuración Actual

### Program.cs (líneas 103-110)
```csharp
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Excursionistas API v1");
        options.RoutePrefix = string.Empty; // Swagger en la raíz
    });
}
```

### ServiceCollectionExtensions.cs (AddSwaggerConfiguration)
```csharp
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Excursionistas API",
        Version = "v1",
        Description = "API para optimización de selección de equipo de escalada...",
        Contact = new()
        {
            Name = "Excursionistas Team",
            Url = new Uri("https://github.com/LozanoAndersonTheStain/prueba-excursionistas")
        }
    });

    // Incluir comentarios XML
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});
```

---

## 🎯 Checklist de Funcionalidades Swagger

- [x] Swagger UI habilitado en Development
- [x] Documentación XML generada
- [x] Comentarios del código integrados
- [x] Modelos con descripciones
- [x] Códigos de respuesta documentados
- [x] Try it out interactivo
- [x] Descarga de swagger.json
- [x] Curl commands generados
- [ ] Swagger en Docker (pendiente)
- [ ] Múltiples ejemplos por endpoint
- [ ] Autenticación JWT
- [ ] Temas personalizados

---

## 🚀 Siguiente Paso

**¡Abre http://localhost:5002 en tu navegador y explora la API!**

Puedes probar todos los endpoints sin escribir código, ver respuestas en tiempo real y entender la estructura completa de la API.

**Desarrollado con ❤️ por Anderson Lozano**
