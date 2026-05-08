# 🏠 SprintRoom — Skill Base

> **Versión:** 1.1  
> **Última actualización:** 2026-05-08  
> **Fuentes:** Notion (SprintRoom workspace), Excalidraw (diagramas de producto), análisis del repositorio  
> **Propósito:** Proporcionar contexto completo y estructurado para que cualquier agente de IA (o humano) entienda el proyecto sin necesidad de explicaciones adicionales.

---

## 1. ¿Qué es SprintRoom?

SprintRoom es una **plataforma de gestión de proyectos ágil** orientada a equipos pequeños. Su filosofía es **simplicidad con control**: eliminar complejidad innecesaria (sin épicas, sin sprints formales) y enfocarse en la ejecución real del trabajo.

### Visión del Producto

- Sistema **cerrado por invitación** — No existe registro público de usuarios.
- Un administrador principal controla quién accede y quién puede invitar.
- Cada proyecto es un **espacio aislado** con su propio contexto, usuarios y progreso.
- La jerarquía es simple y estricta: **Proyecto → User Stories → Tareas**.

---

## 2. Lore del Producto (Flujo Funcional Completo)

### 2.1 Autenticación y Acceso

- **El registro NO es público.** Siempre es mediante invitación.
- Existe un **administrador principal** con control total sobre cuentas.
- El administrador puede crear usuarios y decidir si estos pueden invitar a otros.
- Autenticación basada en **JWT**.

### 2.2 Proyectos

- El administrador (o usuario con permisos) crea un proyecto.
- El creador del proyecto asocia a los demás usuarios participantes.
- Cada proyecto funciona como un **espacio aislado**.

### 2.3 User Stories

- **No se usan épicas.** Se consideran demasiado amplias para equipos pequeños.
- Las user stories son la unidad organizativa dentro de un proyecto.
- Cada user story representa una **unidad clara de funcionalidad**.

### 2.4 Tareas

- Las tareas son la **unidad más granular** del sistema.
- Se crean a partir de user stories.
- Se asignan a usuarios específicos.
- Estados definidos (obligatorios, no inventar nuevos):
  - `NotStarted` (peso: 0) — Sin empezar
  - `InProgress` (peso: 40) — En desarrollo
  - `Testing` (peso: 70) — Probando
  - `Review` (peso: 90) — Revisión
  - `Completed` (peso: 100) — Completado
- Formato de user story: `As a [role], I want to [action] so that [benefit]`

### 2.5 Progreso Automático

El progreso se calcula automáticamente en cascada usando **pesos numéricos por estado**:

1. **Progreso de Tarea** = Peso del estado actual (NotStarted=0, InProgress=40, Testing=70, Review=90, Completed=100)
2. **Progreso de User Story** = `promedio del progreso de sus tareas`
3. **Progreso de Proyecto** = `promedio del progreso de todas sus user stories`

> **Ejemplo:** Una user story con 3 tareas (InProgress=40, Testing=70, Completed=100) → progreso = (40+70+100)/3 = 70%.

### 2.6 Regla de JWT vs userId

- Usar **JWT (token del usuario autenticado)** cuando la acción sea sobre "mí":
  - Mis proyectos, mis tareas, mi perfil.
- Usar **`{userId}` en la ruta** cuando la acción sea sobre otro usuario o una asignación:
  - Agregar usuario a proyecto, quitar usuario de tarea, cambiar rol de usuario, listar tareas de un usuario específico.

### 2.7 Pantallas del Producto (Extraído de Excalidraw)

**Navegación principal:** `SprintRoom | Home | Personal Board | Projects | Task`

| Pantalla | Descripción | Elementos clave |
|----------|-------------|------------------|
| **Login** | Acceso cerrado con campos Email y Password | Botón "Get Started", link a Register |
| **Register** | Registro de nuevos usuarios | Campos: `full name`, `Email`, `password`, `confirm your password` |
| **Create New Project** | Formulario de creación de proyecto | Campos: `Project name *`, `Brief description *`, `Link to repository (optional)`, `Assign Users` (por email), botones Cancel / Create Project |
| **ProjectName (Dashboard)** | Vista principal de un proyecto | Barra de progreso con %, tabs: `Documentation`, `User Histories`, `Backlog`, `Board` |
| **UserHistories** | Listado de user stories de un proyecto | Tabla: `IdHu | HuName | % | Users | Description`, botón `CreateNew HU` |
| **Create New User History** | Modal de creación de HU | Campos: `Enter HU name *`, descripción con formato "As a [role]...", `Assign Users` |
| **List Task In UserStory** | Tareas dentro de una user story | Tabla: `IdTask | TaskName | Status | % | User | Description` |
| **Create New Task** | Modal de creación de tarea | Campos: `Enter Task Name *`, descripción, `Set Difficult`, `Assign Users` |
| **Modal Click On Task** | Detalle de tarea al hacer click | Info completa: TaskName, HU, ProjectName, Status, Priority, Difficulty, Users, Description, Comments |
| **BoardTask** | Vista Kanban de tareas | Columnas por estado: Not Started, In Progress, Testing, Review, Completed |
| **BackLog Task** | Vista tipo backlog | Tabla: `ProjectCode | HuCode | TaskCode | TaskName | Status | % | User | Description` |
| **Personal Board** | Tareas personales del usuario | `MyTask List`, `Process AllMyTasks`, `Personal BoardTask`, `Personal BackLogTask` |
| **Profile** | Perfil del usuario | Nombre, Email, Photo, `password change` |
| **Project Members** | Gestión de usuarios del proyecto | Lista de usuarios, roles (Owner/Admin/Developer/Viewer), `Confirm Remove User`, confirmación con `Write "{UserName}/{ProjectName}"` |

---

## 3. Arquitectura

### 3.1 Patrón Arquitectónico

**DDD + Clean Architecture** — Capas separadas por responsabilidad.

### 3.2 Estructura del Backend

```
SprintRoom.sln
└── src/
    ├── SprintRoom.Api            → Capa de presentación (endpoints HTTP)
    ├── SprintRoom.Application    → Lógica de aplicación (casos de uso, DTOs, interfaces)
    ├── SprintRoom.Domain         → Núcleo del negocio (entidades, enums, reglas)
    └── SprintRoom.Infrastructure → Implementación técnica (EF Core, MySQL, repos)
```

### 3.3 Responsabilidades por Capa

| Capa | Responsabilidad | Contiene |
|------|----------------|----------|
| **Api** | Recibir solicitudes y devolver respuestas | Controllers, Configuración (JWT, middlewares), `appsettings.json` |
| **Application** | Orquestar lógica sin dependencias técnicas | Services (casos de uso), DTOs, Interfaces de repositorios |
| **Domain** | Lógica pura del negocio, sin dependencias externas | Entidades (`Project`, `UserStory`, `ProjectTask`), Enums (`TaskStatus`), Reglas de negocio |
| **Infrastructure** | Conectar el dominio con BD y servicios externos | Entity Framework Core, Configuración MySQL, Implementación de repositorios, Integraciones externas |

### 3.4 Dependencias entre Capas

```
Api → Application, Infrastructure
Application → Domain
Infrastructure → Application, Domain
Domain → (sin dependencias externas)
```

### 3.5 Configuración

- **`appsettings.json`**: Configuración general (ConnectionStrings, JWT Key/Issuer/Audience).
- **`.env`**: Variables sensibles (DB_PASSWORD, JWT_SECRET). **Debe estar en `.gitignore`.**
- **`.env.example`**: Template vacío para referencia.

---

## 4. Stack Tecnológico

### Backend

| Componente | Tecnología |
|-----------|-----------|
| Lenguaje | **C# .NET 8** (SDK `8.0.420`) |
| Framework | **ASP.NET Core Web API** |
| ORM | **Entity Framework Core** |
| Base de datos | **MySQL** (Aiven.io cloud) |
| Autenticación | **JWT** |
| Documentación API | **Swagger** (Swashbuckle.AspNetCore 6.6.2) |

### Frontend (repo separado)

| Componente | Tecnología |
|-----------|-----------|
| Framework | **React** |
| Lenguaje | **TypeScript** |
| Build tool | **Vite** |
| UI/Estilos | **Tailwind CSS** |

### Paquetes NuGet (API)

- `Microsoft.AspNetCore.OpenApi` v8.0.26
- `Swashbuckle.AspNetCore` v6.6.2

---

## 5. Modelo de Datos

### 5.1 Jerarquía Principal

```
Project
 └── UserStory
      └── ProjectTask
```

### 5.2 Entidades

#### User

| Campo | Tipo | Notas |
|-------|------|-------|
| Id | GUID/int | PK |
| Name | string | |
| Email | string | **Único** |
| PasswordHash | string | Contraseña cifrada, NUNCA la real |
| Status | bool | Estado activo/inactivo |
| CreatedAt | DateTime | |
| UpdatedAt | DateTime | |

#### Project

| Campo | Tipo | Notas |
|-------|------|-------|
| Id | GUID/int | PK |
| Code | string | Código identificador |
| Name | string | |
| Description | string | |
| RepositoryUrl | string | URL del repositorio |
| CreatedAt | DateTime | |
| UpdatedAt | DateTime | |

#### UserStory

| Campo | Tipo | Notas |
|-------|------|-------|
| Id | GUID/int | PK |
| Code | string | Código identificador |
| Name | string | |
| Description | string | |
| Priority | enum | Low, Medium, High |
| ProjectId | FK | Referencia a Project |
| CreatedAt | DateTime | |
| UpdatedAt | DateTime | |

#### ProjectTask

| Campo | Tipo | Notas |
|-------|------|-------|
| Id | GUID/int | PK |
| Code | string | Código identificador |
| Name | string | |
| Description | string | |
| Priority | enum | Low, Medium, High |
| Difficulty | enum | VeryLow, Low, Medium, MediumHigh, High |
| Status | enum | NotStarted, InProgress, Testing, Review, Completed |
| UserStoryId | FK | Referencia a UserStory |
| CreatedAt | DateTime | |
| UpdatedAt | DateTime | |

### 5.3 Tablas Intermedias (Asignaciones)

| Tabla | Relación | PK Compuesta | Campos extra |
|-------|----------|-------------|-------------|
| `UserProjectAssignee` | User ↔ Project | UserId + ProjectId | Role |
| `UserStoryAssignee` | User ↔ UserStory | UserId + UserStoryId | — |
| `TaskAssignee` | User ↔ ProjectTask | UserId + TaskId | — |

### 5.4 Enums (con Pesos Numéricos)

```csharp
public enum Priority     { Low = 0,     Medium = 50,     High = 100 }
public enum Difficulty   { VeryLow = 0, Low = 25, Medium = 50, MediumHigh = 75, High = 100 }
public enum TaskStatus   { NotStarted = 0, InProgress = 40, Testing = 70, Review = 90, Completed = 100 }
public enum Role         { Viewer = 10, Developer = 30, Admin = 50, Owner = 100 }
```

> **Regla:** Los enums se almacenan como `INT` en la base de datos. **Nunca** usar strings sueltos para estados.  
> Los pesos numéricos se usan para calcular progreso y determinar jerarquía de permisos.

### 5.5 Relaciones Clave (Extraído de Excalidraw)

- Un **Project** puede tener muchas **UserStories**.
- Un **Project** puede tener muchos **Users**.
- Un **User** puede tener y estar en muchos **Projects**.
- Una **UserStory** puede tener muchas **Tasks**.
- Una **UserStory** puede tener muchos **Users** (del proyecto).
- Una **Task** puede tener muchos **Users** (del proyecto y de la HU).
- Las tablas intermedias usan **PK compuesta** para evitar duplicados.

---

## 6. Base de Datos

### Proveedor

**MySQL** hospedada en **Aiven.io** (cloud).

### Connection String (Estructura)

```
Server=<host>;Port=<port>;Database=<db>;Uid=<user>;Pwd=<password>;AllowPublicKeyRetrieval=True;UseSSL=True;
```

### Configuración en el Proyecto

Se configura en `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Port=...;Database=...;Uid=...;Pwd=...;AllowPublicKeyRetrieval=True;UseSSL=True;"
  }
}
```

> ⚠️ **Nunca** commitear credenciales reales. Usar `.env` o `appsettings.Development.json` (que está en `.gitignore`).

### Naming en BD

- Tablas y columnas: **`snake_case`** recomendado.
- Ejemplos: `projects`, `user_stories`, `tasks`.

---

## 7. Documentación del Backend (Notion)

### Subpáginas de Documentation

| Página | Contenido |
|--------|-----------|
| **BackEnd** | Guías sobre creación de arquitectura base, cálculo de progreso, endpoints |
| **FrontEnd** | (referencia, repo separado) |
| **Architecture** | Estructura DDD Clean Architecture completa |
| **Database** | Credenciales, esquema ER, diagrama de clases, modelo relacional |
| **Tecnologías** | Stack completo del proyecto |
| **Módulos** | (pendiente de contenido) |

### Subpáginas de BackEnd

- **Creación de arquitectura base** — Guía paso a paso
- **Progress Calculation** — Lógica de cálculo automático de progreso
- **EndPoints** — Documentación de endpoints API

---

## 8. Endpoints Planificados (Extraído de Excalidraw)

> **Estado actual:** Solo existe `/ping → pong` (health check). Los endpoints listados abajo son el diseño objetivo.

### 8.1 Auth

| Método | Ruta | Descripción |
|--------|------|-------------|
| `POST` | `/auth/register` | Registrar un nuevo usuario |
| `POST` | `/auth/login` | Generar token JWT de validación |
| `GET` | `/auth/me` | Obtener datos del usuario autenticado usando el token |

### 8.2 Projects

| Método | Ruta | Descripción |
|--------|------|-------------|
| `POST` | `/projects` | Crear un nuevo proyecto |
| `GET` | `/projects` | Listar proyectos visibles para el usuario autenticado |
| `GET` | `/projects/{projectId}` | Consultar un proyecto específico |
| `PUT` | `/projects/{projectId}` | Actualizar un proyecto |
| `DELETE` | `/projects/{projectId}` | Eliminar un proyecto |
| `GET` | `/projects/my` | Listar los proyectos del usuario autenticado |

### 8.3 Project Members

| Método | Ruta | Descripción |
|--------|------|-------------|
| `POST` | `/projects/{projectId}/members` | Asignar un usuario a un proyecto |
| `GET` | `/projects/{projectId}/members` | Listar los usuarios asignados a un proyecto |
| `PUT` | `/projects/{projectId}/members/{userId}/role` | Cambiar el rol de un usuario dentro de un proyecto |
| `DELETE` | `/projects/{projectId}/members/{userId}` | Quitar un usuario de un proyecto |

### 8.4 User Stories

| Método | Ruta | Descripción |
|--------|------|-------------|
| `POST` | `/projects/{projectId}/user-stories` | Crear una historia de usuario dentro de un proyecto |
| `GET` | `/projects/{projectId}/user-stories` | Listar las historias de usuario de un proyecto |
| `GET` | `/user-stories/{userStoryId}` | Consultar una historia de usuario específica |
| `PUT` | `/user-stories/{userStoryId}` | Actualizar una historia de usuario |
| `DELETE` | `/user-stories/{userStoryId}` | Eliminar una historia de usuario |

### 8.5 User Story Assignees

| Método | Ruta | Descripción |
|--------|------|-------------|
| `POST` | `/user-stories/{userStoryId}/assignees` | Asignar un usuario a una historia de usuario |
| `GET` | `/user-stories/{userStoryId}/assignees` | Listar usuarios asignados a una historia de usuario |
| `DELETE` | `/user-stories/{userStoryId}/assignees/{userId}` | Quitar un usuario de una historia de usuario |

### 8.6 Tasks

| Método | Ruta | Descripción |
|--------|------|-------------|
| `POST` | `/user-stories/{userStoryId}/tasks` | Crear una tarea dentro de una historia de usuario |
| `GET` | `/user-stories/{userStoryId}/tasks` | Listar tareas de una historia de usuario |
| `GET` | `/tasks/{taskId}` | Consultar una tarea específica |
| `PUT` | `/tasks/{taskId}` | Actualizar una tarea |
| `PATCH` | `/tasks/{taskId}/status` | Cambiar solo el estado de una tarea |
| `DELETE` | `/tasks/{taskId}` | Eliminar una tarea |

### 8.7 Task Assignees

| Método | Ruta | Descripción |
|--------|------|-------------|
| `POST` | `/tasks/{taskId}/assignees` | Asignar un usuario a una tarea |
| `GET` | `/tasks/{taskId}/assignees` | Listar usuarios asignados a una tarea |
| `DELETE` | `/tasks/{taskId}/assignees/{userId}` | Quitar un usuario de una tarea |

### 8.8 Progress

| Método | Ruta | Descripción |
|--------|------|-------------|
| `GET` | `/projects/{projectId}/progress` | Consultar el progreso general de un proyecto |
| `GET` | `/user-stories/{userStoryId}/progress` | Consultar el progreso de una historia de usuario |

---

## 9. Entry Point

```csharp
// src/SprintRoom.Api/Program.cs
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapGet("/ping", () => "pong");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Run();
```

---

## 10. Ideas y Notas del Equipo

- **Paletas de colores atractivas** para el frontend y diseño del producto (página Ideas en Notion).
- El diseño visual se trabaja en **Excalidraw** con flujos y wireframes.
- Slogan del producto: **"Organize fast. Build faster"** (by RuntimeStudioDev).
- Las tareas muestran info resumida: `Completed: N° | Pending: N°`.
