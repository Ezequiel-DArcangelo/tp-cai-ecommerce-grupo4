# E-commerce con arquitectura de microservicios

Trabajo Práctico de la materia **Construcción de Aplicaciones Informáticas (CAI)** — Licenciatura en Sistemas de Información de las Organizaciones, Facultad de Ciencias Económicas (UBA).

Aplicación de e-commerce construida con cinco microservicios independientes escritos en C# / .NET 8, cada uno con su propia base de datos SQLite. La comunicación entre microservicios se realiza vía HTTP en tiempo de ejecución.

---

## Tabla de contenidos

- [Integrantes](#integrantes)
- [Estructura del repositorio](#estructura-del-repositorio)
- [Stack técnico](#stack-técnico)
- [Microservicios y puertos](#microservicios-y-puertos)
- [Requisitos previos](#requisitos-previos)
- [Cómo correr el proyecto](#cómo-correr-el-proyecto)
- [Endpoints por microservicio](#endpoints-por-microservicio)
- [Catálogo de códigos de error](#catálogo-de-códigos-de-error)
- [Health Checks](#health-checks)
- [Logging y trazabilidad](#logging-y-trazabilidad)
- [Documentación adicional](#documentación-adicional)

---

## Integrantes

| Nombre | Microservicios |
|---|---|
| Ezequiel D'Arcangelo | Users |
| Laura Moggia | Orders, Cart |
| Stéfano Rago | Products, Notifications |

---

## Estructura del repositorio

```
tp-cai-ecommerce-grupo4/
├── src/
│   ├── Users.API/           # Gestión de usuarios (registro, login, consulta)
│   ├── Products.API/        # Catálogo de productos y stock
│   ├── Orders.API/          # Órdenes de compra
│   ├── Cart.API/            # Carrito de compras
│   └── Notifications.API/   # Notificaciones a usuarios
├── docs/                    # Diagramas y capturas de Swagger
├── ECommerce/
│   └── ECommerce.slnx       # Solución que referencia los 5 microservicios
├── .gitignore
└── README.md
```

Cada microservicio sigue una estructura similar:

```
<Servicio>.API/
├── Controllers/            # Endpoints HTTP
├── Data/                   # Acceso a base SQLite (Dapper)
├── DTOs/                   # Objetos de transferencia de datos
├── Exceptions/             # Excepciones de dominio (NotFound, BusinessRule, Validation)
├── ExceptionHandlers/      # Handlers que traducen excepciones a Problem Details
├── Middlewares/            # Correlation ID y auditoría
├── Models/                 # Entidades del dominio
├── Repositories/           # Repositorios con queries SQL
├── Services/               # Lógica de negocio
├── appsettings.json        # Configuración (connection string, URLs de otros micros)
└── Program.cs              # Configuración de la aplicación
```

---

## Stack técnico

- **.NET 8** (ASP.NET Core Web API)
- **SQLite** + **Dapper** para persistencia (una base por microservicio)
- **BCrypt.Net-Next** para hash de passwords (en Users)
- **Serilog** para logging estructurado en consola y archivos JSON
- **Swashbuckle (Swagger UI)** para documentación de APIs
- **Microsoft.Extensions.Diagnostics.HealthChecks** para endpoints de salud
- **IHttpClientFactory** para comunicación entre microservicios

---

## Microservicios y puertos

Cada microservicio corre en su propio puerto HTTPS. Los puertos están fijados en `launchSettings.json` de cada proyecto.

| Microservicio | URL Base | Swagger UI |
|---|---|---|
| Users.API | `https://localhost:7206` | `https://localhost:7206/swagger` |
| Products.API | `https://localhost:7196` | `https://localhost:7196/swagger` |
| Orders.API | `https://localhost:56970` | `https://localhost:56970/swagger` |
| Cart.API | `https://localhost:7002` | `https://localhost:7002/swagger` |
| Notifications.API | `https://localhost:7244` | `https://localhost:7244/swagger` |

---

## Requisitos previos

Para correr el proyecto se necesita tener instalado en la máquina:

1. **.NET 8 SDK**
   Descarga: [https://dotnet.microsoft.com/download/dotnet/8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
   Verificar instalación con: `dotnet --version` (debe mostrar `8.x.x`)

2. **Visual Studio 2022** (Community es suficiente)
   Con el workload de **ASP.NET y desarrollo web** habilitado.
   Alternativa: VS Code + extensión de C#, pero la guía está pensada para Visual Studio.

3. **Git** (para clonar el repositorio)

No hace falta instalar SQLite por separado: el proyecto usa el paquete NuGet `Microsoft.Data.Sqlite` que incluye todo lo necesario. Las bases se crean automáticamente al arrancar cada microservicio.

---

## Cómo correr el proyecto

### Paso 1: Clonar el repositorio

```bash
git clone https://github.com/Ezequiel-DArcangelo/tp-cai-ecommerce-grupo4.git
cd tp-cai-ecommerce-grupo4
```

### Paso 2: Abrir la solución en Visual Studio

1. Entrar a la carpeta `ECommerce/` dentro del repo y hacer doble clic en `ECommerce.slnx`. También se puede abrir desde Visual Studio con `Archivo > Abrir > Proyecto/Solución` y navegar al archivo.

2. **Importante**: el formato `.slnx` requiere Visual Studio 2022 versión **17.10 o superior**. Si la versión instalada es anterior, actualizar Visual Studio o, alternativamente, usar `dotnet build` desde la línea de comandos en la raíz del repo.

3. Visual Studio va a cargar los 5 proyectos. Esperar a que termine de restaurar paquetes NuGet (se ve abajo en la barra de estado).

### Paso 3: Compilar la solución

En Visual Studio:
- Menú: **Compilar → Recompilar solución** (o `Ctrl+Shift+B`)
- Debe compilar sin errores. Pueden aparecer warnings, son normales.

### Paso 4: Configurar múltiples proyectos de inicio

Esto permite ejecutar los 5 microservicios a la vez (necesario para probar la integración).

1. En el **Solution Explorer**, **clic derecho sobre la solución `ECommerce`** (el nodo de arriba de todo)
2. Seleccionar **"Establecer proyectos de inicio..."** (Set Startup Projects...)
3. Elegir **"Multiple startup projects"** (Múltiples proyectos de inicio)
4. Para cada uno de los 5 proyectos, cambiar la columna **Acción** a **"Iniciar"** (Start):
   - Cart.API
   - Notifications.API
   - Orders.API
   - Products.API
   - Users.API
5. Aceptar

### Paso 5: Ejecutar

Apretar **F5** (o el botón "Iniciar" arriba en Visual Studio).

Esperado:
- Se abren **5 ventanas de consola** (una por microservicio), cada una mostrando los logs de Serilog
- Se abren **5 pestañas en el navegador** con la Swagger UI de cada microservicio
- Cada Swagger debería responder y permitir hacer pruebas

### Paso 6: Verificar que todo arrancó

En cada Swagger, hacer un GET al endpoint `/health`. Debe devolver:

```json
{
  "status": "Healthy",
  "checks": [ ... ]
}
```

Si todos los `/health` responden Healthy, el sistema está listo para usarse.

---

## Endpoints por microservicio

### Users.API

| Método | Ruta | Descripción |
|---|---|---|
| POST | `/api/users/register` | Registra un nuevo usuario |
| POST | `/api/users/login` | Autentica un usuario |
| GET | `/api/users/{id}` | Obtiene un usuario por ID |
| GET | `/health` | Estado general del servicio |
| GET | `/health/ready` | Indica si está listo para recibir tráfico |
| GET | `/health/live` | Indica si el proceso responde |

### Products.API

| Método | Ruta | Descripción |
|---|---|---|
| POST | `/api/products` | Crea un nuevo producto |
| GET | `/api/products` | Lista todos los productos |
| GET | `/api/products/{id}` | Obtiene un producto por ID |
| PUT | `/api/products/{id}/stock` | Actualiza el stock de un producto |

### Orders.API

| Método | Ruta | Descripción |
|---|---|---|
| POST | `/api/orders` | Crea una nueva orden (valida usuario y productos) |
| GET | `/api/orders` | Lista órdenes (filtro opcional por usuario) |
| GET | `/api/orders/{id}` | Obtiene una orden por ID |
| PATCH | `/api/orders/{id}/status` | Actualiza el estado de una orden |

### Cart.API

| Método | Ruta | Descripción |
|---|---|---|
| POST | `/api/cart` | Agrega un ítem al carrito |
| GET | `/api/cart/{usuarioId}` | Obtiene el carrito de un usuario |
| DELETE | `/api/cart/{usuarioId}/items/{productoId}` | Elimina un ítem del carrito |

### Notifications.API

| Método | Ruta | Descripción |
|---|---|---|
| POST | `/api/notifications/send` | Envía una notificación a un usuario |
| GET | `/api/notifications/{usuarioId}` | Lista notificaciones de un usuario |

Para detalle completo de cada endpoint (parámetros, body, ejemplos), ver Swagger UI de cada microservicio.

---

## Catálogo de códigos de error

Cada microservicio tiene su propio prefijo de códigos de error. Las respuestas de error siguen el formato Problem Details (RFC 7807) extendido con `errorCode` y `errorMessage`.

### Users (USR)

| Código | Status | Descripción |
|---|---|---|
| USR-001 | 409 | Email ya registrado |
| USR-002 | 400 | Datos inválidos en el registro |
| USR-003 | 401 | Credenciales incorrectas |
| USR-004 | 403 | Usuario bloqueado por intentos fallidos |
| USR-005 | 403 | Usuario bloqueado por fraude |
| USR-006 | 500 | Error interno del servidor |
| USR-007 | 404 | Usuario no encontrado |

### Products (PRD)

| Código | Status | Descripción |
|---|---|---|
| PRD-001 | 404 | Producto no encontrado |
| PRD-002 | 400 | Datos inválidos del producto |
| PRD-003 | 409 | Producto duplicado |
| PRD-004 | 400 | Stock insuficiente |
| PRD-005 | 500 | Error interno del servidor |

### Orders (ORD)

| Código | Status | Descripción |
|---|---|---|
| ORD-001 | 404 | Orden no encontrada |
| ORD-002 | 400 | Datos inválidos de la orden |
| ORD-003 | 404 | Usuario no existe (al validar contra Users) |
| ORD-004 | 404 | Producto no existe (al validar contra Products) |
| ORD-005 | 409 | Stock insuficiente para algún producto |
| ORD-006 | 409 | Transición de estado inválida |
| ORD-007 | 500 | Error al comunicarse con otro microservicio |

### Cart (CRT)

| Código | Status | Descripción |
|---|---|---|
| CRT-001 | 404 | Carrito no encontrado |
| CRT-002 | 400 | Datos inválidos |
| CRT-003 | 404 | Ítem no encontrado en el carrito |

### Notifications (NTF)

| Código | Status | Descripción |
|---|---|---|
| NTF-001 | 404 | Usuario no encontrado (al validar contra Users) |
| NTF-002 | 400 | Datos inválidos de la notificación |
| NTF-003 | 500 | Error interno del servidor |

---

## Health Checks

Cada microservicio expone tres endpoints de health check:

- **`/health`**: corre todos los checks registrados. Devuelve `Healthy` si todo funciona, `Unhealthy` si algo falla.
- **`/health/ready`**: solo corre los checks etiquetados como `ready` (típicamente la conexión a SQLite). Indica si el servicio está listo para recibir tráfico.
- **`/health/live`**: no corre ningún check. Solo responde para indicar que el proceso está vivo.

La respuesta es JSON con el siguiente formato:

```json
{
  "status": "Healthy",
  "checks": [
    {
      "name": "sqlite",
      "status": "Healthy",
      "description": "La base de datos SQLite responde correctamente."
    }
  ],
  "totalDuration": "00:00:00.0123456"
}
```

---

## Logging y trazabilidad

### Serilog

Cada microservicio usa Serilog con dos sinks:

- **Console**: logs en formato legible para ver en la terminal mientras se desarrolla
- **File**: logs en formato JSON en la carpeta `logs/` con rotación diaria (archivos tipo `log-YYYYMMDD.txt`)

Cada log incluye: timestamp, nivel, nombre del servicio, mensaje, y propiedades estructuradas.

### Correlation ID

Cada request HTTP recibe un identificador único (`X-Correlation-Id`) que se propaga entre microservicios:

- Si el request llega con el header `X-Correlation-Id`, se respeta y se reutiliza.
- Si no, se genera un Guid nuevo.
- El ID se devuelve en el header de la respuesta y se incluye en todos los logs del request.

Esto permite rastrear una operación completa que toque varios microservicios buscando el mismo ID en los logs de cada uno. Por ejemplo, un POST a Orders genera logs en Orders, en Users (cuando Orders valida el usuario) y en Products (cuando Orders valida los productos), todos con el mismo Correlation ID.

---

## Documentación adicional

- **Swagger UI** de cada microservicio: ver columna "Swagger UI" en la tabla de [Microservicios y puertos](#microservicios-y-puertos)
- **Diagrama de arquitectura**: `docs/arquitectura.png`
- **Capturas de Swagger**: carpeta `docs/swagger/`

---

## Notas técnicas

- Las bases SQLite se generan automáticamente al arrancar cada microservicio (archivo `<servicio>.db` en la raíz del proyecto). Están ignoradas por `.gitignore` para no subirse al repo.
- Las carpetas `logs/` que generan los logs de Serilog también están ignoradas.
- Para limpiar las bases y los logs y partir de cero, simplemente borrar los archivos correspondientes y volver a arrancar cada microservicio.
