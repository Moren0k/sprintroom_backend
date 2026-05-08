# 🤖 AGENTS.md — Reglas para Agentes de IA en SprintRoom

> **Versión:** 1.1  
> **Última actualización:** 2026-05-08  
> **Aplica a:** Todo agente de IA que trabaje en este repositorio.

---

## 0. Lectura Obligatoria

Antes de cualquier acción, **lee completo** `.skill/sprintroom.md`. Ese archivo contiene el lore, la arquitectura, el modelo de datos, el stack tecnológico y las decisiones de diseño del proyecto.

---

## 1. Reglas Generales de Comportamiento

### 1.1 No modificar estructura sin autorización

- **NUNCA** reorganizar carpetas, renombrar proyectos ni mover archivos entre capas.
- La estructura `src/{Api, Application, Domain, Infrastructure}` es sagrada.
- No crear proyectos nuevos en la solución sin aprobación explícita.

### 1.2 No ampliar alcance sin acuerdo

- No agregar features, endpoints ni entidades que no hayan sido solicitadas.
- Si detectas algo que "sería buena idea agregar", **proponlo primero, no lo implementes**.
- Todo cambio de alcance se habla primero (**Regla 1 del equipo**).

### 1.3 Siempre trabajar sobre una tarea concreta

- Cada trabajo debe estar asociado a una **user story** y una **tarea específica**.
- No trabajar "en general". Si no hay tarea definida, pedir clarificación.
- (**Regla 2 del equipo**).

### 1.4 Una sola fuente de verdad

- La documentación vive en `.skill/` y en Notion.
- No duplicar documentación en otros lugares.
- (**Regla 3 del equipo**).

---

## 2. Reglas de Código (Naming Conventions)

### 2.1 C# — Convenciones Estrictas

| Elemento | Convención | Ejemplo |
|----------|-----------|---------|
| Clases | `PascalCase` | `ProjectService` |
| Interfaces | `I` + `PascalCase` | `IUserRepository` |
| Métodos | `PascalCase` | `GetProjectName()` |
| Propiedades | `PascalCase` | `TaskCount` |
| Variables / Parámetros | `camelCase` | `projectName`, `totalTasks` |
| Campos privados | `_camelCase` | `_projectRepository`, `_taskCount` |
| Constantes | `PascalCase` | `MaxTasksPerUser` |
| Enums (nombre y valores) | `PascalCase` | `TaskStatus.InProgress` |

### 2.2 Prohibiciones Explícitas

- ❌ **Nunca** `snake_case` ni `camelCase` para clases, métodos o propiedades.
- ❌ **Nunca** `SCREAMING_CASE` para constantes (ej: `MAX_TASKS`). Usar `PascalCase`.
- ❌ **Nunca** strings sueltos para estados → usar **enums siempre**.

### 2.3 Archivos

- Nombre del archivo **= nombre de la clase**.
- **1 clase principal por archivo**.
- Ejemplo: `ProjectService.cs` contiene `public class ProjectService`.

### 2.4 Base de Datos (MySQL)

- Tablas y columnas: **`snake_case`** recomendado.
- Ejemplos: `projects`, `user_stories`, `tasks`.
- Los enums se almacenan como **`INT`** en la BD.

---

## 3. Reglas de Arquitectura

### 3.1 Flujo de Dependencias (Obligatorio)

```
Api → Application, Infrastructure
Application → Domain
Infrastructure → Application, Domain
Domain → SIN DEPENDENCIAS EXTERNAS
```

### 3.2 Reglas por Capa

#### Domain (Núcleo)

- **Solo** entidades, enums, value objects y reglas de negocio.
- **Sin** referencias a Entity Framework, HTTP, o cualquier infraestructura.
- **Sin** DTOs — los DTOs viven en Application.

#### Application

- Contiene **services** (casos de uso), **DTOs**, e **interfaces de repositorios**.
- Orquesta la lógica **sin depender de detalles técnicos**.
- Puede referenciar Domain, **nunca** Infrastructure ni Api.

#### Infrastructure

- Implementa las **interfaces** definidas en Application.
- Contiene **DbContext**, configuraciones de EF Core, implementaciones de repositorios.
- Puede referenciar Application y Domain.

#### Api

- **Solo** controllers, configuración (JWT, middlewares) y `Program.cs`.
- Nunca lógica de negocio en controllers.
- Puede referenciar Application e Infrastructure.

---

## 4. Reglas de Git y Workflow

### 4.1 Ramas

- **Nunca** trabajar directo sobre `main` o `develop`.
- Formato de rama: `usuario/tk-00-tipo-descripcion`
  - Ejemplo: `david/tk-01-feature-crear-proyecto`
- Todas las ramas salen de `develop`.

### 4.2 Commits

- Formato: `tipo: descripción corta`
- Tipos válidos: `feat`, `fix`, `refactor`, `docs`, `style`, `test`, `chore`
- Ejemplo: `feat: agregar endpoint de creación de proyecto`

### 4.3 Flujo

1. Crear rama desde `develop`.
2. Trabajar en la rama.
3. Abrir PR hacia `develop`.
4. Review y merge.
5. `main` solo recibe merges de `develop` cuando hay una versión estable.

---

## 5. Reglas de Estados de Tareas

Los **únicos** estados válidos para tareas son:

| Enum Value | Display Name |
|-----------|-------------|
| `NotStarted` | Sin empezar |
| `InProgress` | En desarrollo |
| `Testing` | Probando |
| `Review` | Revisión |
| `Completed` | Completado |

- **No inventar estados nuevos** sin definirlos formalmente.
- Una tarea **no** está "completada" solo porque "ya casi" → debe cumplir lo acordado y estar realmente terminada (**Definition of Done**).

---

## 6. Reglas de Documentación y Comentarios

### 6.1 Código 100% Comentado

- Todo bloque de código debe estar comentado de manera **efectiva y clara**.
- Explicar: **Para qué**, **Por qué** y **Qué hace** (**Regla 9 del equipo**).
- No comentarios triviales (`// incrementa i`) → comentarios de propósito.

- Cuando implementes algo nuevo, **evalúa el impacto** según el *Flujo de mejora continua* (Sección 11) y actualiza `.skill/sprintroom.md` si es necesario.
- Documenta decisiones técnicas importantes que no sean evidentes por el código solo.

---

## 7. Reglas de Seguridad

- **Nunca** commitear credenciales, passwords ni tokens.
- Variables sensibles van en `.env` (que está en `.gitignore`).
- Usar `.env.example` como template sin valores reales.
- Las passwords se almacenan como **hash**, nunca en texto plano.

---

## 8. Reglas de Tareas

- Las tareas deben ser **pequeñas**: entenderse rápido, terminarse en poco tiempo, revisarse fácil.
- Si una tarea parece enorme, **está mal definida** → dividirla (**Regla 6 del equipo**).
- Reuniones solo para: **desbloqueos**, **decisiones**, **asignación** (**Regla 8 del equipo**).

---

## 9. Enums del Dominio (Referencia Rápida)

```csharp
public enum Priority     { Low, Medium, High }
public enum Difficulty   { VeryLow, Low, Medium, MediumHigh, High }
public enum TaskStatus   { NotStarted, InProgress, Testing, Review, Completed }
public enum Role         { Viewer, Developer, Admin, Owner }
```

---

## 11. Flujo de mejora continua de la skill

Al finalizar cada tarea, el agente **debe** realizar una revisión breve de impacto para decidir si actualiza `.skill/sprintroom.md`.

### 11.1 Sistema de Clasificación de Importancia

| Nivel | Descripción | Acción |
|-------|-------------|--------|
| **Bajo** | Cambios puntuales, temporales, corrección de typos o cambios evidentes. | **No actualizar** la skill. |
| **Medio** | Cambios en convenciones, estructura de carpetas, comportamiento recurrente o decisiones útiles para futuros agentes. | **Evaluar** si aporta valor a largo plazo. |
| **Alto** | Cambios en arquitectura, reglas de negocio, flujos principales, entidades clave, integraciones o decisiones de diseño base. | **Actualizar obligatoriamente** la skill. |

### 11.2 Checklist de Impacto (Revisión Final)

Responde estas preguntas mentalmente al terminar tu tarea:
1. ¿Lo que hice cambia o aclara una **regla de negocio**?
2. ¿Introduce una **nueva convención** técnica?
3. ¿Afecta **arquitectura**, carpetas, módulos o responsabilidades?
4. ¿Modifica un **flujo importante** del usuario o del sistema?
5. ¿Agrega una **integración, dependencia o servicio** relevante?
6. ¿Corrige una **confusión** que futuros agentes podrían repetir?
7. ¿Preserva o cambia una **decisión de diseño** importante?

> [!IMPORTANT]
> **Regla de Actualización:** Si respondiste **SÍ** a una o más preguntas y el impacto es **Medio** o **Alto**, debes actualizar `.skill/sprintroom.md` antes de cerrar la tarea.

### 11.3 Reglas de Control de Documentación
- **No documentar cambios triviales**: Mantener la skill limpia.
- **No duplicar**: Si ya está en Notion y es estable, solo referenciar.
- **No inventar**: Si no estás seguro de una regla, consúltala o márcala como `[SUPUESTO]`.
- **Registrar conflictos**: Si tu implementación contradice algo en la skill, documenta el cambio y por qué ocurrió.
- **Accionable**: La skill debe ser breve y útil para que el próximo agente actúe rápido.

---

## 12. Checklist Pre-Commit para Agentes

Antes de proponer cualquier cambio, verifica:

- [ ] ¿Sigue las convenciones de naming? (PascalCase/camelCase/_camelCase)
- [ ] ¿Respeta la separación de capas? (Domain sin dependencias externas)
- [ ] ¿El código está comentado explicando propósito?
- [ ] ¿No amplía el alcance sin autorización?
- [ ] ¿No incluye credenciales ni secrets?
- [ ] ¿El nombre del archivo coincide con la clase principal?
- [ ] ¿Los estados de tareas usan los enums definidos?
- [ ] ¿El commit message sigue el formato `tipo: descripción`?
- [ ] **¿Evalué el impacto en la skill y actualicé `sprintroom.md` si era Nivel Medio/Alto?**

---

## 13. Contexto del Equipo

- **Equipo:** Pequeño (2-3 desarrolladores).
- **Comunicación:** Notion como fuente de verdad. No decisiones importantes perdidas en WhatsApp.
- **Herramientas de diseño:** Excalidraw para wireframes y flujos.
- **Filosofía:** Simplicidad > Complejidad. Ejecución real > Planificación excesiva.

