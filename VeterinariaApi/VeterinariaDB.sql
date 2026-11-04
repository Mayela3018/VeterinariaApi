/* =========================================================
   Script: VeterinariaDB
   Proyecto: VeterinariaApi - Web API REST (Semana 12)
   Relación: Duenos (1) ---- (N) Mascotas
   ========================================================= */

/* ---------------------------------------------------------
   PARTE A: Crear la base de datos (SOLO EN LOCAL)
   En la nube NO se ejecuta esta parte, porque el hosting
   ya te da la base creada.
   --------------------------------------------------------- */
IF DB_ID('VeterinariaDB') IS NULL
    CREATE DATABASE VeterinariaDB;
GO

USE VeterinariaDB;
GO

/* ---------------------------------------------------------
   PARTE B: Tablas, restricciones y datos de ejemplo
   (esta parte SÍ se ejecuta también en la nube)
   --------------------------------------------------------- */

IF OBJECT_ID('dbo.Mascotas', 'U') IS NOT NULL DROP TABLE dbo.Mascotas;
IF OBJECT_ID('dbo.Duenos', 'U') IS NOT NULL DROP TABLE dbo.Duenos;
GO

CREATE TABLE dbo.Duenos (
    Id         INT IDENTITY(1,1) NOT NULL,
    Nombres    NVARCHAR(100)     NOT NULL,
    Dni        CHAR(8)           NOT NULL,
    Telefono   VARCHAR(15)       NOT NULL,
    Email      NVARCHAR(100)     NULL,
    CONSTRAINT PK_Duenos PRIMARY KEY (Id),
    CONSTRAINT UQ_Duenos_Dni UNIQUE (Dni)
);
GO

CREATE TABLE dbo.Mascotas (
    Id         INT IDENTITY(1,1) NOT NULL,
    Nombre     NVARCHAR(50)      NOT NULL,
    Especie    NVARCHAR(30)      NOT NULL,
    Raza       NVARCHAR(50)      NULL,
    Edad       INT               NOT NULL,
    Peso       DECIMAL(5,2)      NOT NULL,
    DuenoId    INT               NOT NULL,
    CONSTRAINT PK_Mascotas PRIMARY KEY (Id),
    CONSTRAINT FK_Mascotas_Duenos FOREIGN KEY (DuenoId)
        REFERENCES dbo.Duenos (Id),
    CONSTRAINT CK_Mascotas_Edad CHECK (Edad BETWEEN 0 AND 40),
    CONSTRAINT CK_Mascotas_Peso CHECK (Peso > 0)
);
GO

/* Datos de ejemplo: Dueños */
INSERT INTO dbo.Duenos (Nombres, Dni, Telefono, Email) VALUES
(N'Lucía Fernández Ramos',  '71234567', '987654321', N'lucia.fernandez@gmail.com'),
(N'Carlos Quispe Mamani',   '72345678', '976543210', N'carlos.quispe@gmail.com'),
(N'Andrea Torres Salazar',  '73456789', '965432109', N'andrea.torres@hotmail.com'),
(N'Jorge Huamán Ccori',     '74567890', '954321098', NULL),
(N'Valeria Rojas Paredes',  '75678901', '943210987', N'valeria.rojas@gmail.com');
GO

/* Datos de ejemplo: Mascotas
   (el dueño 5 se queda SIN mascotas para probar el DELETE 204) */
INSERT INTO dbo.Mascotas (Nombre, Especie, Raza, Edad, Peso, DuenoId) VALUES
(N'Firulais', N'Perro',  N'Labrador',        5, 28.50, 1),
(N'Michi',    N'Gato',   N'Siamés',          3,  4.20, 1),
(N'Rocky',    N'Perro',  N'Bulldog Francés', 2, 11.80, 2),
(N'Luna',     N'Gato',   N'Persa',           4,  3.90, 2),
(N'Toby',     N'Perro',  N'Mestizo',         7, 15.30, 3),
(N'Copito',   N'Conejo', N'Angora',          1,  1.60, 3),
(N'Kiwi',     N'Ave',    N'Periquito',       2,  0.05, 4),
(N'Max',      N'Perro',  N'Pastor Alemán',   6, 32.00, 4);
GO

/* Verificación */
SELECT * FROM dbo.Duenos;
SELECT * FROM dbo.Mascotas;
GO