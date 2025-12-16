/* ===========================================================
   RAC0CA / AYUDA SOCIAL - BD FINAL SIMPLE (EVENTOS PRESENCIALES)
   - Nombres en snake_case
   - Sin duplicados
   - Orden: DB -> DROPS -> CREATES -> INDEXES -> SEED
   =========================================================== */

-- 0) Crear BD si no existe
IF DB_ID('Racoca') IS NULL
BEGIN
    CREATE DATABASE Racoca;
END
GO

USE Racoca;
GO

/* ===========================================================
   1) DROPS (en orden por dependencias)
   =========================================================== */
IF OBJECT_ID('dbo.evento_inscripciones', 'U') IS NOT NULL DROP TABLE dbo.evento_inscripciones;
IF OBJECT_ID('dbo.evento_imagenes', 'U') IS NOT NULL DROP TABLE dbo.evento_imagenes;
IF OBJECT_ID('dbo.eventos', 'U') IS NOT NULL DROP TABLE dbo.eventos;

IF OBJECT_ID('dbo.organizacion_redes_sociales', 'U') IS NOT NULL DROP TABLE dbo.organizacion_redes_sociales;
IF OBJECT_ID('dbo.organizacion_perfil', 'U') IS NOT NULL DROP TABLE dbo.organizacion_perfil;

IF OBJECT_ID('dbo.voluntario_habilidad', 'U') IS NOT NULL DROP TABLE dbo.voluntario_habilidad;
IF OBJECT_ID('dbo.voluntario_perfil', 'U') IS NOT NULL DROP TABLE dbo.voluntario_perfil;

IF OBJECT_ID('dbo.distritos', 'U') IS NOT NULL DROP TABLE dbo.distritos;
IF OBJECT_ID('dbo.tipos_organizacion', 'U') IS NOT NULL DROP TABLE dbo.tipos_organizacion;
IF OBJECT_ID('dbo.categorias_evento', 'U') IS NOT NULL DROP TABLE dbo.categorias_evento;
IF OBJECT_ID('dbo.habilidades', 'U') IS NOT NULL DROP TABLE dbo.habilidades;
IF OBJECT_ID('dbo.usuarios', 'U') IS NOT NULL DROP TABLE dbo.usuarios;
GO

/* ===========================================================
   2) TABLAS BASE
   =========================================================== */

-- 2.1 Usuarios (login)
CREATE TABLE dbo.usuarios (
    id               INT IDENTITY(1,1) NOT NULL,
    email            NVARCHAR(255)     NOT NULL,
    password_hash    NVARCHAR(255)     NOT NULL,
    rol              NVARCHAR(20)      NOT NULL,  -- VOLUNTARIO / ORGANIZACION
    es_mayor_edad    BIT               NOT NULL,
    estado           NVARCHAR(20)      NOT NULL DEFAULT N'ACTIVO',
    fecha_creacion   DATETIME          NOT NULL DEFAULT(GETDATE()),
    foto_perfil_url  NVARCHAR(500)     NULL,

    CONSTRAINT PK_usuarios PRIMARY KEY (id),
    CONSTRAINT UQ_usuarios_email UNIQUE (email),
    CONSTRAINT CK_usuarios_rol CHECK (rol IN (N'VOLUNTARIO', N'ORGANIZACION')),
    CONSTRAINT CK_usuarios_estado CHECK (estado IN (N'ACTIVO', N'INACTIVO'))
);
GO

-- 2.2 Cat logo habilidades
CREATE TABLE dbo.habilidades (
    id     INT IDENTITY(1,1) NOT NULL,
    nombre NVARCHAR(100)     NOT NULL,

    CONSTRAINT PK_habilidades PRIMARY KEY (id),
    CONSTRAINT UQ_habilidades_nombre UNIQUE (nombre)
);
GO

-- 2.3 Tipos de organizaci n (cat logo)
CREATE TABLE dbo.tipos_organizacion (
    id     INT IDENTITY(1,1) NOT NULL,
    nombre NVARCHAR(100)     NOT NULL,

    CONSTRAINT PK_tipos_organizacion PRIMARY KEY (id),
    CONSTRAINT UQ_tipos_organizacion_nombre UNIQUE (nombre)
);
GO

-- 2.4 Distritos (cat logo)
CREATE TABLE dbo.distritos (
    id     INT IDENTITY(1,1) NOT NULL,
    nombre NVARCHAR(100)     NOT NULL,
    activo BIT              NOT NULL DEFAULT(1),

    CONSTRAINT PK_distritos PRIMARY KEY (id),
    CONSTRAINT UQ_distritos_nombre UNIQUE (nombre)
);
GO

-- 2.5 Categor as de evento (cat logo)
CREATE TABLE dbo.categorias_evento (
    id     INT IDENTITY(1,1) NOT NULL,
    nombre NVARCHAR(255)     NOT NULL,

    CONSTRAINT PK_categorias_evento PRIMARY KEY (id),
    CONSTRAINT UQ_categorias_evento_nombre UNIQUE (nombre)
);
GO

/* ===========================================================
   3) PERFILES
   =========================================================== */

-- 3.1 Perfil voluntario
CREATE TABLE dbo.voluntario_perfil (
    usuario_id       INT NOT NULL,
    nombres          NVARCHAR(100) NOT NULL,
    apellidos        NVARCHAR(100) NOT NULL,
    sexo             NVARCHAR(20)  NULL,
    celular          NVARCHAR(20)  NULL,
    fecha_nacimiento DATE         NULL,
    ciudad           NVARCHAR(100) NULL,
    distrito         NVARCHAR(100) NULL,

    CONSTRAINT PK_voluntario_perfil PRIMARY KEY (usuario_id),
    CONSTRAINT FK_voluntario_usuario FOREIGN KEY (usuario_id)
        REFERENCES dbo.usuarios(id)
);
GO

CREATE TABLE dbo.voluntario_habilidad (
    usuario_id   INT NOT NULL,
    habilidad_id INT NOT NULL,

    CONSTRAINT PK_voluntario_habilidad PRIMARY KEY (usuario_id, habilidad_id),
    CONSTRAINT FK_vh_usuario FOREIGN KEY (usuario_id)
        REFERENCES dbo.usuarios(id),
    CONSTRAINT FK_vh_habilidad FOREIGN KEY (habilidad_id)
        REFERENCES dbo.habilidades(id)
);
GO

-- 3.2 Perfil organizaci n
CREATE TABLE dbo.organizacion_perfil (
    usuario_id            INT NOT NULL,

    nombres_responsable   NVARCHAR(100) NOT NULL,
    apellidos_responsable NVARCHAR(100) NOT NULL,
    celular_responsable   NVARCHAR(20)  NULL,

    nombre_organizacion   NVARCHAR(255) NOT NULL,
    tipo_organizacion     NVARCHAR(100) NOT NULL, 
    descripcion_corta     NVARCHAR(300) NULL,

    ciudad                NVARCHAR(100) NULL,
    distrito              NVARCHAR(100) NULL,
    direccion             NVARCHAR(255) NULL,

    CONSTRAINT PK_organizacion_perfil PRIMARY KEY (usuario_id),
    CONSTRAINT FK_organizacion_usuario FOREIGN KEY (usuario_id)
        REFERENCES dbo.usuarios(id)
);
GO

CREATE TABLE dbo.organizacion_redes_sociales (
    id         INT IDENTITY(1,1) NOT NULL,
    usuario_id INT NOT NULL,
    plataforma NVARCHAR(30) NOT NULL, -- FB, IG, TT, X, WEB, etc.
    url        NVARCHAR(500) NOT NULL,

    CONSTRAINT PK_organizacion_redes_sociales PRIMARY KEY (id),
    CONSTRAINT FK_redes_org FOREIGN KEY (usuario_id)
        REFERENCES dbo.usuarios(id)
);
GO

/* ===========================================================
   4) EVENTOS
   =========================================================== */
CREATE TABLE dbo.eventos (
    id INT IDENTITY(1,1) NOT NULL,

    organizacion_id INT NOT NULL,  -- FK a usuarios(id)
    categoria_id    INT NOT NULL,  -- FK a categorias_evento(id)

    nombre            NVARCHAR(255) NOT NULL,
    descripcion_corta NVARCHAR(300) NOT NULL,
    descripcion_larga NVARCHAR(MAX) NOT NULL,

    fecha_creacion DATETIME NOT NULL DEFAULT(GETDATE()),
    fecha_evento   DATE     NOT NULL,
    hora_evento    TIME     NOT NULL,

    ciudad    NVARCHAR(100) NOT NULL,
    distrito  NVARCHAR(100) NOT NULL,
    direccion NVARCHAR(255) NOT NULL,

    mapa_url NVARCHAR(500) NULL,

    ilimitado    BIT NOT NULL DEFAULT(1),
    cupos_limite INT NULL,

    requisitos NVARCHAR(MAX) NULL,
    incluye    NVARCHAR(MAX) NULL,
    no_incluye NVARCHAR(MAX) NULL,

    metodo_contacto NVARCHAR(20) NOT NULL DEFAULT(N'WHATSAPP'),
    whatsapp_numero NVARCHAR(20) NULL,

    imagen_principal NVARCHAR(500) NOT NULL,

    estado NVARCHAR(20) NOT NULL DEFAULT(N'PUBLICADO'),

    CONSTRAINT PK_eventos PRIMARY KEY (id),
    CONSTRAINT FK_eventos_org FOREIGN KEY (organizacion_id)
        REFERENCES dbo.usuarios(id),
    CONSTRAINT FK_eventos_cat FOREIGN KEY (categoria_id)
        REFERENCES dbo.categorias_evento(id),

    CONSTRAINT CK_eventos_metodo_contacto
        CHECK (metodo_contacto IN (N'WHATSAPP', N'CHAT', N'AMBOS')),

    CONSTRAINT CK_eventos_estado
        CHECK (estado IN (N'PUBLICADO', N'PAUSADO', N'CERRADO')),

    CONSTRAINT CK_eventos_cupos
        CHECK (
            (ilimitado = 1 AND cupos_limite IS NULL)
            OR
            (ilimitado = 0 AND cupos_limite IS NOT NULL AND cupos_limite > 0)
        )
);
GO

CREATE TABLE dbo.evento_imagenes (
    id        INT IDENTITY(1,1) NOT NULL,
    evento_id INT NOT NULL,
    url       NVARCHAR(500) NOT NULL,

    CONSTRAINT PK_evento_imagenes PRIMARY KEY (id),
    CONSTRAINT FK_evento_imagenes_evento FOREIGN KEY (evento_id)
        REFERENCES dbo.eventos(id)
);
GO

CREATE TABLE dbo.evento_inscripciones (
    id           INT IDENTITY(1,1) NOT NULL,
    evento_id     INT NOT NULL,
    voluntario_id INT NOT NULL,

    fecha  DATETIME     NOT NULL DEFAULT(GETDATE()),
    estado NVARCHAR(20) NOT NULL DEFAULT(N'INSCRITO'),

    CONSTRAINT PK_evento_inscripciones PRIMARY KEY (id),
    CONSTRAINT FK_insc_event FOREIGN KEY (evento_id)
        REFERENCES dbo.eventos(id),
    CONSTRAINT FK_insc_vol FOREIGN KEY (voluntario_id)
        REFERENCES dbo.usuarios(id),

    CONSTRAINT UQ_evento_inscripciones UNIQUE (evento_id, voluntario_id),
    CONSTRAINT CK_insc_estado CHECK (estado IN (N'INSCRITO', N'APROBADO', N'RECHAZADO', N'CANCELADO'))
);
GO

/* ===========================================================
   5)  NDICES
   =========================================================== */
CREATE INDEX IX_eventos_filtros
ON dbo.eventos (distrito, categoria_id, fecha_evento, estado);
GO

CREATE INDEX IX_inscripciones_evento
ON dbo.evento_inscripciones (evento_id, estado);
GO

/* ===========================================================
   6) SEED (DATOS INICIALES)
   =========================================================== */

-- Categor as
INSERT INTO dbo.categorias_evento(nombre) VALUES
(N'Ni ez'),(N'Adulto mayor'),(N'Animales'),(N'Salud'),(N'Medio ambiente');
GO

-- Habilidades
INSERT INTO dbo.habilidades(nombre) VALUES
(N'Organizaci n'),(N'Comunicaci n'),(N'Primeros auxilios'),
(N'Cocina'),(N'Limpieza'),(N'Log stica'),(N'Fotograf a'),(N'Dise o');
GO

-- Tipos organizaci n
INSERT INTO dbo.tipos_organizacion(nombre) VALUES
(N'ONG'),
(N'Asociaci n'),
(N'Comunidad'),
(N'Iglesia / Comunidad religiosa'),
(N'Municipalidad / Entidad p blica'),
(N'Universidad / Instituto'),
(N'Empresa (RSE)'),
(N'Fundaci n'),
(N'Colectivo'),
(N'Otro');
GO

-- Distritos (Lima)
INSERT INTO dbo.distritos(nombre) VALUES
(N'Anc n'),(N'Ate'),(N'Barranco'),(N'Bre a'),(N'Carabayllo'),
(N'Chaclacayo'),(N'Chorrillos'),(N'Cieneguilla'),(N'Comas'),(N'El Agustino'),
(N'Independencia'),(N'Jes s Mar a'),(N'La Molina'),(N'La Victoria'),(N'Lima'),
(N'Lince'),(N'Los Olivos'),(N'Lurigancho'),(N'Lur n'),(N'Magdalena del Mar'),
(N'Miraflores'),(N'Pachac mac'),(N'Pucusana'),(N'Pueblo Libre'),
(N'Puente Piedra'),(N'Punta Hermosa'),(N'Punta Negra'),(N'R mac'),
(N'San Bartolo'),(N'San Borja'),(N'San Isidro'),(N'San Juan de Lurigancho'),
(N'San Juan de Miraflores'),(N'San Luis'),(N'San Mart n de Porres'),
(N'San Miguel'),(N'Santa Anita'),(N'Santa Mar a del Mar'),(N'Santa Rosa'),
(N'Santiago de Surco'),(N'Surquillo'),(N'Villa El Salvador'),(N'Villa Mar a del Triunfo');
GO

-- Usuario organizaci n demo
INSERT INTO dbo.usuarios(email, password_hash, rol, es_mayor_edad, estado, foto_perfil_url)
VALUES (N'org@demo.com', N'HASH_DEMO', N'ORGANIZACION', 1, N'ACTIVO', N'https://picsum.photos/300/300');
GO

INSERT INTO dbo.organizacion_perfil(
 usuario_id, nombres_responsable, apellidos_responsable, celular_responsable,
 nombre_organizacion, tipo_organizacion, descripcion_corta,
 ciudad, distrito, direccion
)
VALUES (
 1, N'Ana', N'P rez', N'999999999',
 N'ONG Demo', N'ONG', N'Ayuda social en Lima',
 N'Lima', N'Miraflores', N'Av. Demo 123'
);
GO

-- Usuario voluntario demo
INSERT INTO dbo.usuarios(email, password_hash, rol, es_mayor_edad, estado, foto_perfil_url)
VALUES (N'vol@demo.com', N'HASH_DEMO', N'VOLUNTARIO', 1, N'ACTIVO', NULL);
GO

INSERT INTO dbo.voluntario_perfil(usuario_id, nombres, apellidos, sexo, celular, fecha_nacimiento, ciudad, distrito)
VALUES (2, N'Luis', N'G mez', N'M', N'988888888', '2000-01-01', N'Lima', N'San Miguel');
GO


ALTER TABLE dbo.eventos
ALTER COLUMN imagen_principal NVARCHAR(255) NULL;



SELECT name, definition
FROM sys.check_constraints
WHERE name = 'CK_eventos_estado';


SELECT dc.name, dc.definition
FROM sys.default_constraints dc
JOIN sys.columns c ON c.default_object_id = dc.object_id
JOIN sys.tables t ON t.object_id = c.object_id
WHERE t.name='eventos' AND c.name='estado';
USE Racoca;
SELECT * FROM dbo.usuarios;
SELECT * FROM dbo.voluntario_perfil;
SELECT u.id, u.email, u.rol, u.foto_perfil_url,
       vp.nombres, vp.apellidos, vp.celular, vp.ciudad, vp.distrito, vp.fecha_nacimiento
FROM dbo.usuarios u
JOIN dbo.voluntario_perfil vp ON vp.usuario_id = u.id
WHERE u.id = 2; -- tu voluntario demo

INSERT INTO dbo.usuarios(email, password_hash, rol, es_mayor_edad, estado, foto_perfil_url)
VALUES (N'piero.onocuica@gmail.com', N'HASH_PIERO', N'VOLUNTARIO', 1, N'ACTIVO',
        N'https://picsum.photos/300/300?random=77');
GO

DECLARE @PieroId INT = SCOPE_IDENTITY();
SELECT @PieroId AS PieroId;
GO

DECLARE @PieroId INT = (SELECT TOP 1 id FROM dbo.usuarios WHERE email = N'piero@demo.com');

INSERT INTO dbo.voluntario_perfil(usuario_id, nombres, apellidos, sexo, celular, fecha_nacimiento, ciudad, distrito)
VALUES (@PieroId, N'Piero Carlos', N'Onocuica Maza', N'M', N'921926452', '2001-11-11', N'Lima', N'Independencia');
GO

-- Asegura que existan
IF NOT EXISTS (SELECT 1 FROM dbo.habilidades WHERE nombre = N'Comunicación')
    INSERT INTO dbo.habilidades(nombre) VALUES (N'Comunicación');

IF NOT EXISTS (SELECT 1 FROM dbo.habilidades WHERE nombre = N'Organización')
    INSERT INTO dbo.habilidades(nombre) VALUES (N'Organización');

IF NOT EXISTS (SELECT 1 FROM dbo.habilidades WHERE nombre = N'Logística')
    INSERT INTO dbo.habilidades(nombre) VALUES (N'Logística');

IF NOT EXISTS (SELECT 1 FROM dbo.habilidades WHERE nombre = N'Primeros auxilios')
    INSERT INTO dbo.habilidades(nombre) VALUES (N'Primeros auxilios');
GO

DECLARE @PieroId INT = (SELECT TOP 1 id FROM dbo.usuarios WHERE email = N'piero@demo.com');

INSERT INTO dbo.voluntario_habilidad(usuario_id, habilidad_id)
SELECT @PieroId, h.id
FROM dbo.habilidades h
WHERE h.nombre IN (N'Comunicación', N'Organización', N'Logística', N'Primeros auxilios')
  AND NOT EXISTS (
      SELECT 1 FROM dbo.voluntario_habilidad vh
      WHERE vh.usuario_id = @PieroId AND vh.habilidad_id = h.id
  );
GO

USE Racoca;
GO

UPDATE dbo.usuarios
SET 
    password_hash = N'POM123456'
WHERE     email = N'piero.onocuica@gmail.com'

GO

