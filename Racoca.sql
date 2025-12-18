/* ===========================================================
   RAC0CA / AYUDA SOCIAL - BD FINAL (EVENTOS PRESENCIALES)
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

-- 2.2 Catálogo habilidades
CREATE TABLE dbo.habilidades (
    id     INT IDENTITY(1,1) NOT NULL,
    nombre NVARCHAR(100)     NOT NULL,

    CONSTRAINT PK_habilidades PRIMARY KEY (id),
    CONSTRAINT UQ_habilidades_nombre UNIQUE (nombre)
);
GO

-- 2.3 Tipos de organización (catálogo)
CREATE TABLE dbo.tipos_organizacion (
    id     INT IDENTITY(1,1) NOT NULL,
    nombre NVARCHAR(100)     NOT NULL,

    CONSTRAINT PK_tipos_organizacion PRIMARY KEY (id),
    CONSTRAINT UQ_tipos_organizacion_nombre UNIQUE (nombre)
);
GO

-- 2.4 Distritos (catálogo)
CREATE TABLE dbo.distritos (
    id     INT IDENTITY(1,1) NOT NULL,
    nombre NVARCHAR(100)     NOT NULL,
    activo BIT              NOT NULL DEFAULT(1),

    CONSTRAINT PK_distritos PRIMARY KEY (id),
    CONSTRAINT UQ_distritos_nombre UNIQUE (nombre)
);
GO

-- 2.5 Categorías de evento (catálogo)
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

-- 3.2 Perfil organización
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
    plataforma NVARCHAR(30) NOT NULL,
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

    organizacion_id INT NOT NULL,
    categoria_id    INT NOT NULL,

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

    imagen_principal NVARCHAR(500) NULL, -- (dejas NULL permitido si quieres)
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
   5) ÍNDICES
   =========================================================== */
CREATE INDEX IX_eventos_filtros
ON dbo.eventos (distrito, categoria_id, fecha_evento, estado);
GO

CREATE INDEX IX_inscripciones_evento
ON dbo.evento_inscripciones (evento_id, estado);
GO

/* ===========================================================
   6) SEED (CATÁLOGOS)
   =========================================================== */

INSERT INTO dbo.categorias_evento(nombre) VALUES
(N'Educación'),(N'Nutrición'),(N'Deporte'),(N'Salud'),(N'Medio ambiente'),(N'Cultura');
GO

INSERT INTO dbo.habilidades(nombre) VALUES
(N'Organización'),(N'Comunicación'),(N'Primeros auxilios'),
(N'Cocina'),(N'Limpieza'),(N'Logística'),(N'Fotografía'),(N'Diseño');
GO

INSERT INTO dbo.tipos_organizacion(nombre) VALUES
(N'ONG'),(N'Asociación'),(N'Comunidad'),(N'Iglesia / Comunidad religiosa'),
(N'Municipalidad / Entidad pública'),(N'Universidad / Instituto'),
(N'Empresa (RSE)'),(N'Fundación'),(N'Colectivo'),(N'Otro');
GO

INSERT INTO dbo.distritos(nombre) VALUES
(N'Miraflores'),(N'Pueblo Libre'),(N'San Juan de Miraflores'),
(N'Los Olivos'),(N'Santiago de Surco'),(N'Magdalena del Mar'),(N'Barranco');
GO

/* ===========================================================
   7) ORGANIZACIÓN REAL
   =========================================================== */
INSERT INTO dbo.usuarios(email, password_hash, rol, es_mayor_edad, estado, foto_perfil_url)
VALUES (N'contacto@unidos.pe', N'HASH_PENDIENTE', N'ORGANIZACION', 1, N'ACTIVO', N'/uploads/avatars/org_default.png');
GO

DECLARE @OrgId INT = (SELECT TOP 1 id FROM dbo.usuarios WHERE email = N'contacto@unidos.pe');

INSERT INTO dbo.organizacion_perfil(
 usuario_id, nombres_responsable, apellidos_responsable, celular_responsable,
 nombre_organizacion, tipo_organizacion, descripcion_corta,
 ciudad, distrito, direccion
)
VALUES (
 @OrgId, N'Carolina', N'Mendoza', N'987654321',
 N'Manos Unidas Perú', N'ONG',
 N'Voluntariado en salud, nutrición, educación, deporte, cultura y medio ambiente.',
 N'Lima', N'Miraflores', N'Av. Arequipa 1850'
);
GO

/* ===========================================================
   8) EVENTOS DEMO (6 eventos con imágenes locales)
   =========================================================== */
DECLARE @OrgId2 INT = (SELECT TOP 1 id FROM dbo.usuarios WHERE email = N'contacto@unidos.pe');

DECLARE @CatSalud INT     = (SELECT TOP 1 id FROM dbo.categorias_evento WHERE nombre = N'Salud');
DECLARE @CatNutri INT     = (SELECT TOP 1 id FROM dbo.categorias_evento WHERE nombre = N'Nutrición');
DECLARE @CatEdu INT       = (SELECT TOP 1 id FROM dbo.categorias_evento WHERE nombre = N'Educación');
DECLARE @CatDep INT       = (SELECT TOP 1 id FROM dbo.categorias_evento WHERE nombre = N'Deporte');
DECLARE @CatCult INT      = (SELECT TOP 1 id FROM dbo.categorias_evento WHERE nombre = N'Cultura');
DECLARE @CatMedio INT     = (SELECT TOP 1 id FROM dbo.categorias_evento WHERE nombre = N'Medio ambiente');

INSERT INTO dbo.eventos (
 organizacion_id, categoria_id,
 nombre, descripcion_corta, descripcion_larga,
 fecha_evento, hora_evento,
 ciudad, distrito, direccion, mapa_url,
 ilimitado, cupos_limite,
 requisitos, incluye, no_incluye,
 metodo_contacto, whatsapp_numero,
 imagen_principal, estado
)
VALUES
(@OrgId2, @CatSalud,
 N'Campaña de salud preventiva',
 N'Apoyo en registro, orden y orientación a familias.',
 N'Jornada con charlas preventivas, apoyo en logística y distribución de material informativo.',
 '2026-01-22','09:00',
 N'Lima',N'Pueblo Libre',N'Plaza Bolívar (Pueblo Libre)',NULL,
 0,30,
 N'Puntualidad y actitud de servicio', N'Constancia digital', N'Atención médica especializada',
 N'WHATSAPP',N'987654321',
 N'/uploads/eventos/salud.jpg',N'PUBLICADO'),

(@OrgId2, @CatNutri,
 N'Olla común: apoyo y nutrición',
 N'Preparación, empaque y entrega de raciones saludables.',
 N'Apoyo en cocina y distribución. Orientación básica sobre hábitos saludables.',
 '2026-01-25','08:00',
 N'Lima',N'San Juan de Miraflores',N'Local comunal SJM',NULL,
 0,25,
 N'Cabello recogido y manos limpias', N'Guantes y mascarilla', N'Pasajes',
 N'WHATSAPP',N'987654321',
 N'/uploads/eventos/nutricion.jpg',N'PUBLICADO'),

(@OrgId2, @CatEdu,
 N'Refuerzo escolar (lectura y matemática)',
 N'Acompañamiento educativo a niños.',
 N'Dinámicas de lectura, ejercicios de matemática básica y apoyo personalizado.',
 '2026-01-27','16:00',
 N'Lima',N'Los Olivos',N'Casa de la Juventud - Los Olivos',NULL,
 0,20,
 N'Paciencia y compromiso', N'Material impreso', N'Certificado físico',
 N'WHATSAPP',N'987654321',
 N'/uploads/eventos/educacion.jpg',N'PUBLICADO'),

(@OrgId2, @CatDep,
 N'Jornada deportiva comunitaria',
 N'Apoyo en coordinación y control de asistencia.',
 N'Organización de grupos, hidratación, apoyo logístico y dinámicas deportivas para adolescentes.',
 '2026-01-29','15:30',
 N'Lima',N'Santiago de Surco',N'Complejo deportivo municipal - Surco',NULL,
 1,NULL,
 N'Ropa cómoda', N'Agua para voluntarios', N'Implementos personales',
 N'WHATSAPP',N'987654321',
 N'/uploads/eventos/deporte.jpg',N'PUBLICADO'),

(@OrgId2, @CatCult,
 N'Taller cultural (arte y lectura)',
 N'Dinámicas culturales para niños y familias.',
 N'Apoyo en lectura guiada, dibujo, teatro corto y actividades de integración comunitaria.',
 '2026-02-01','10:00',
 N'Lima',N'Magdalena del Mar',N'Centro cultural vecinal - Magdalena',NULL,
 0,30,
 N'Ganas de participar', N'Material básico', N'Alimentos',
 N'WHATSAPP',N'987654321',
 N'/uploads/eventos/cultura.jpg',N'PUBLICADO'),

(@OrgId2, @CatMedio,
 N'Limpieza de parque y reciclaje',
 N'Recolección y segregación de residuos.',
 N'Actividad de limpieza con puntos de acopio y orientación para reciclaje.',
 '2026-02-03','08:30',
 N'Lima',N'Barranco',N'Parque Municipal - Barranco',NULL,
 1,NULL,
 N'Bloqueador y gorra', N'Bolsas y guantes (si tienes)', N'Bebidas alcohólicas',
 N'WHATSAPP',N'987654321',
 N'/uploads/eventos/medioambiente.jpg',N'PUBLICADO');
GO

-- Verificación
SELECT TOP 50 id, nombre, distrito, fecha_evento, imagen_principal, estado
FROM dbo.eventos
ORDER BY id DESC;
GO
