# 🧪 Guía de Pruebas - Excursionistas API

## 📍 Estado Actual
- ✅ API corriendo en: **http://localhost:5000**
- ✅ Base de datos SQLite con 20 elementos precargados
- ✅ Algoritmo de optimización funcional

---

## 🚀 Pruebas Rápidas

### 1️⃣ Verificar que la API está Activa

#### PowerShell:
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/elements" -Method Get | Select-Object -First 3
```

#### Navegador:
Abre en tu navegador: http://localhost:5000/api/elements

#### Curl (Git Bash o WSL):
```bash
curl http://localhost:5000/api/elements
```

**Resultado esperado:** Lista de 20 elementos con nombre, peso, calorías y eficiencia calórica

---

## 📋 Pruebas de Endpoints CRUD

### 2️⃣ Obtener Todos los Elementos
```powershell
# Ver todos los elementos en formato tabla
Invoke-RestMethod -Uri "http://localhost:5000/api/elements" | 
    Select-Object id, name, weight, calories, calorieEfficiency | 
    Format-Table
```

### 3️⃣ Obtener un Elemento por ID
```powershell
# Obtener elemento con ID 1
Invoke-RestMethod -Uri "http://localhost:5000/api/elements/1"
```

**Resultado esperado:**
```json
{
  "id": 1,
  "name": "Botella de agua",
  "weight": 1.5,
  "calories": 500.0,
  "calorieEfficiency": 333.33,
  "createdAt": "2026-03-08T...",
  "isActive": true
}
```

### 4️⃣ Crear un Nuevo Elemento
```powershell
$newElement = @{
    name = "Gel Energético"
    weight = 0.05
    calories = 120
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/elements" `
    -Method Post `
    -Body $newElement `
    -ContentType "application/json"
```

**Resultado esperado:** Código 201 Created con el elemento creado

### 5️⃣ Actualizar un Elemento
```powershell
# Actualizar el elemento que acabamos de crear (ajusta el ID según corresponda)
$updateElement = @{
    name = "Gel Energético Premium"
    weight = 0.06
    calories = 150
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/elements/21" `
    -Method Put `
    -Body $updateElement `
    -ContentType "application/json"
```

### 6️⃣ Eliminar un Elemento (Soft Delete)
```powershell
# Eliminar elemento con ID 21
Invoke-RestMethod -Uri "http://localhost:5000/api/elements/21" -Method Delete
```

**Resultado esperado:** Código 204 No Content

---

## 🎯 Pruebas del Algoritmo de Optimización

### 7️⃣ Optimización Básica

**Escenario:** Necesitas 1500 calorías con máximo 5 kg de peso

```powershell
$optimization = @{
    minimumCalories = 1500
    maximumWeight = 5
    elementIds = @(1, 2, 3, 4, 5, 6, 7)
} | ConvertTo-Json

$result = Invoke-RestMethod -Uri "http://localhost:5000/api/optimization/calculate" `
    -Method Post `
    -Body $optimization `
    -ContentType "application/json"

# Ver resultado
$result | ConvertTo-Json -Depth 3

# Ver solo elementos seleccionados
$result.selectedElements | Select-Object name, weight, calories | Format-Table
```

**Resultado esperado:**
```
success: true
totalCalories: >= 1500
totalWeight: <= 5
selectedElements: Lista de 3-5 elementos óptimos ordenados por eficiencia
```

### 8️⃣ Optimización con Todos los Elementos

```powershell
# Usar todos los elementos disponibles (dejar elementIds vacío)
$optimizationAll = @{
    minimumCalories = 2000
    maximumWeight = 3
    elementIds = @()  # Vacío = usar todos
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/optimization/calculate" `
    -Method Post `
    -Body $optimizationAll `
    -ContentType "application/json" | 
    Select-Object success, totalWeight, totalCalories, itemCount
```

### 9️⃣ Optimización Imposible (Sin Solución)

**Escenario:** Requisitos imposibles de cumplir

```powershell
$impossible = @{
    minimumCalories = 10000  # Muy alto
    maximumWeight = 0.5      # Muy bajo
    elementIds = @()
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5000/api/optimization/calculate" `
    -Method Post `
    -Body $impossible `
    -ContentType "application/json"
```

**Resultado esperado:**
```json
{
  "success": false,
  "message": "No se encontró una solución viable...",
  "selectedElements": [],
  "totalWeight": 0,
  "totalCalories": 0
}
```

---

## 🧩 Escenarios de Prueba Reales

### Escenario 1: Excursión Ligera (Día)
```powershell
# 1200 calorías, máximo 2 kg
$lightHike = @{
    minimumCalories = 1200
    maximumWeight = 2
    elementIds = @()
} | ConvertTo-Json

$result = Invoke-RestMethod -Uri "http://localhost:5000/api/optimization/calculate" `
    -Method Post -Body $lightHike -ContentType "application/json"

Write-Host "=== EXCURSIÓN LIGERA ===" -ForegroundColor Cyan
Write-Host "Elementos seleccionados: $($result.itemCount)" -ForegroundColor Green
Write-Host "Peso total: $($result.totalWeight) kg" -ForegroundColor Yellow
Write-Host "Calorías totales: $($result.totalCalories)" -ForegroundColor Magenta
$result.selectedElements | Select-Object name, weight, calories | Format-Table
```

### Escenario 2: Excursión de Montaña (Varios días)
```powershell
# 3000 calorías, máximo 8 kg
$mountainHike = @{
    minimumCalories = 3000
    maximumWeight = 8
    elementIds = @()
} | ConvertTo-Json

$result = Invoke-RestMethod -Uri "http://localhost:5000/api/optimization/calculate" `
    -Method Post -Body $mountainHike -ContentType "application/json"

Write-Host "=== EXCURSIÓN DE MONTAÑA ===" -ForegroundColor Cyan
Write-Host "Elementos seleccionados: $($result.itemCount)" -ForegroundColor Green
Write-Host "Peso total: $($result.totalWeight) kg" -ForegroundColor Yellow
Write-Host "Calorías totales: $($result.totalCalories)" -ForegroundColor Magenta
Write-Host "Eficiencia promedio: $($result.averageEfficiency)" -ForegroundColor White
$result.selectedElements | Select-Object name, weight, calories, calorieEfficiency | Format-Table
```

### Escenario 3: Ultra-Ligero (Minimalista)
```powershell
# 1000 calorías, máximo 0.5 kg
$ultralight = @{
    minimumCalories = 1000
    maximumWeight = 0.5
    elementIds = @()
} | ConvertTo-Json

$result = Invoke-RestMethod -Uri "http://localhost:5000/api/optimization/calculate" `
    -Method Post -Body $ultralight -ContentType "application/json"

Write-Host "=== ULTRA-LIGERO ===" -ForegroundColor Cyan
if ($result.success) {
    Write-Host "✅ SOLUCIÓN ENCONTRADA" -ForegroundColor Green
    $result.selectedElements | Select-Object name, weight, calories | Format-Table
} else {
    Write-Host "❌ NO HAY SOLUCIÓN VIABLE" -ForegroundColor Red
    Write-Host $result.message
}
```

---

## 🔍 Pruebas de Validación

### 10️⃣ Validación: Nombre Vacío (Error Esperado)
```powershell
$invalid = @{
    name = ""
    weight = 1
    calories = 100
} | ConvertTo-Json

try {
    Invoke-RestMethod -Uri "http://localhost:5000/api/elements" `
        -Method Post -Body $invalid -ContentType "application/json"
} catch {
    Write-Host "❌ Error esperado:" -ForegroundColor Red
    $_.Exception.Response
}
```

### 11️⃣ Validación: Peso Negativo (Error Esperado)
```powershell
$invalid = @{
    name = "Test"
    weight = -1
    calories = 100
} | ConvertTo-Json

try {
    Invoke-RestMethod -Uri "http://localhost:5000/api/elements" `
        -Method Post -Body $invalid -ContentType "application/json"
} catch {
    Write-Host "❌ Error de validación capturado correctamente" -ForegroundColor Yellow
}
```

---

## 📊 Análisis de Datos

### Ver Elementos Ordenados por Eficiencia
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/elements" | 
    Where-Object { $_.calories -gt 0 } |
    Sort-Object -Property calorieEfficiency -Descending |
    Select-Object name, weight, calories, calorieEfficiency |
    Format-Table -AutoSize
```

### Ver Solo Elementos con Calorías
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/elements" | 
    Where-Object { $_.calories -gt 0 } |
    Select-Object name, calories, weight |
    Format-Table
```

### Contar Elementos por Categoría
```powershell
$elements = Invoke-RestMethod -Uri "http://localhost:5000/api/elements"
$withCalories = ($elements | Where-Object { $_.calories -gt 0 }).Count
$equipment = ($elements | Where-Object { $_.calories -eq 0 }).Count

Write-Host "Elementos con calorías: $withCalories" -ForegroundColor Green
Write-Host "Equipo sin calorías: $equipment" -ForegroundColor Cyan
Write-Host "Total elementos: $($elements.Count)" -ForegroundColor Yellow
```

---

## 🐛 Pruebas de Manejo de Errores

### Elemento No Encontrado (404)
```powershell
try {
    Invoke-RestMethod -Uri "http://localhost:5000/api/elements/99999"
} catch {
    Write-Host "✅ Error 404 manejado correctamente" -ForegroundColor Green
}
```

### Nombre Duplicado (409 Conflict)
```powershell
$duplicate = @{
    name = "Botella de agua"  # Ya existe
    weight = 1
    calories = 0
} | ConvertTo-Json

try {
    Invoke-RestMethod -Uri "http://localhost:5000/api/elements" `
        -Method Post -Body $duplicate -ContentType "application/json"
} catch {
    Write-Host "✅ Error 409 Conflict manejado" -ForegroundColor Green
}
```

---

## 🔧 Comandos de Docker

### Ver logs en tiempo real
```powershell
docker logs -f excursionistas-api
```

### Detener y reiniciar
```powershell
# Detener
docker-compose down

# Iniciar
docker-compose up -d

# Ver estado
docker ps
```

### Limpiar base de datos y reiniciar
```powershell
# Detener contenedor
docker-compose down

# Eliminar base de datos
Remove-Item -Path ./data/* -Force

# Reiniciar (se recreará con seeding)
docker-compose up -d
Start-Sleep -Seconds 5
docker logs excursionistas-api
```

---

## 🎯 Checklist de Funcionalidades

Marca cada funcionalidad que pruebes:

- [ ] GET todos los elementos
- [ ] GET elemento por ID
- [ ] POST crear elemento
- [ ] PUT actualizar elemento
- [ ] DELETE eliminar elemento
- [ ] Optimización con IDs específicos
- [ ] Optimización con todos los elementos
- [ ] Optimización sin solución viable
- [ ] Validación de campos requeridos
- [ ] Validación de valores negativos
- [ ] Manejo de errores 404
- [ ] Manejo de conflictos 409
- [ ] Análisis de eficiencia calórica
- [ ] Persistencia de datos en volumen

---

## 💡 Tips

1. **Ver respuestas bonitas:** Usa `| ConvertTo-Json -Depth 3` al final
2. **Formato tabla:** Usa `| Format-Table -AutoSize` para tablas
3. **Capturar resultado:** Usa `$result = Invoke-RestMethod ...` para análisis
4. **Filtrar propiedades:** Usa `| Select-Object name, weight, calories`
5. **Ordenar:** Usa `| Sort-Object -Property calorieEfficiency -Descending`

---

## 🚨 Solución de Problemas

### Si la API no responde:
```powershell
docker ps  # Verificar que esté corriendo
docker logs excursionistas-api  # Ver errores
docker-compose restart  # Reiniciar
```

### Si necesitas datos frescos:
```powershell
docker-compose down
Remove-Item -Path ./data/* -Force
docker-compose up -d
```

### Ver consumo de recursos:
```powershell
docker stats excursionistas-api --no-stream
```

---

**¡Disfruta probando la aplicación! 🎉**
