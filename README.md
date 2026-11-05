# 🐶 VeterinariaApi – Web API REST

Web API REST desarrollada con **ASP.NET Core (.NET 10)** y **Entity Framework Core (Database First)** para gestionar los **dueños** y las **mascotas** de una clínica veterinaria.

Laboratorio Semana 12 – Desarrollo de Aplicaciones Empresariales Avanzadas – Tecsup.

## 🌐 API publicada
- **Swagger:** http://veterinariaapi-may.runasp.net/swagger
- **Hosting:** MonsterASP.NET (plan Free, datacenter EU)
- **Base de datos:** SQL Server 2025 en MonsterASP.NET (plan Free)

## 🧱 Modelo de datos
- **Duenos** (1) ──── (N) **Mascotas**, con la clave foránea `Mascotas.DuenoId`
- Script de creación y datos de ejemplo: [`VeterinariaApi/VeterinariaDB.sql`](VeterinariaApi/VeterinariaDB.sql)

## 📌 Endpoints (11)
| Método | Ruta | Descripción |
|---|---|---|
| GET | `/api/Duenos?nombre=` | Lista dueños (filtro por nombre) |
| GET | `/api/Duenos/{id}` | Obtiene un dueño |
| GET | `/api/Duenos/{id}/mascotas` | Mascotas de un dueño (ruta anidada) |
| POST | `/api/Duenos` | Crea un dueño |
| PUT | `/api/Duenos/{id}` | Actualiza un dueño |
| DELETE | `/api/Duenos/{id}` | Elimina un dueño |
| GET | `/api/Mascotas?especie=&duenoId=&nombre=&edadMin=&edadMax=` | Lista mascotas con filtros |
| GET | `/api/Mascotas/{id}` | Obtiene una mascota con su dueño |
| POST | `/api/Mascotas` | Crea una mascota (solo envía `duenoId`) |
| PUT | `/api/Mascotas/{id}` | Actualiza una mascota |
| DELETE | `/api/Mascotas/{id}` | Elimina una mascota |

## ⚖️ Reglas de negocio (409 Conflict)
1. El **DNI** del dueño no se puede repetir.
2. No se puede **eliminar un dueño que tiene mascotas** registradas.
3. Un dueño no puede tener **dos mascotas con el mismo nombre**.

## 🛠️ Tecnologías
ASP.NET Core Web API (.NET 10) · Entity Framework Core 10 (SQL Server) · LINQ async · Data Annotations · OpenAPI + Swagger UI · CORS

## 🔒 Configuración
La cadena de conexión **no está en el repositorio**:
- **Local:** User Secrets (`ConnectionStrings:VeterinariaDB`)
- **Nube:** variable de entorno `ConnectionStrings__VeterinariaDB` en MonsterASP.NET

## ▶️ Ejecutar en local
1. Ejecutar `VeterinariaDB.sql` en SQL Server.
2. Configurar el secreto de usuario `ConnectionStrings:VeterinariaDB`.
3. Ejecutar el proyecto y abrir `https://localhost:7078/swagger`.

---
👩‍💻 Mayela Ticona Mamani
