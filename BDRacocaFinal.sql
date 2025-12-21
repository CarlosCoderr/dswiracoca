IF DB_ID('RacocaFinal') IS NOT NULL
BEGIN
    ALTER DATABASE RacocaFinal SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE RacocaFinal;
END;
GO

CREATE DATABASE RacocaFinal;
GO

USE RacocaFinal;
GO

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

CREATE TABLE dbo.usuarios (
    id               INT IDENTITY(1,1) NOT NULL,
    email            NVARCHAR(255)     NOT NULL,
    password_hash    NVARCHAR(255)     NOT NULL,
    rol              NVARCHAR(20)      NOT NULL,  -- VOLUNTARIO / ORGANIZACION / ADMIN
    es_mayor_edad    BIT               NOT NULL,
    estado           NVARCHAR(20)      NOT NULL DEFAULT N'ACTIVO',
    fecha_creacion   DATETIME          NOT NULL DEFAULT(GETDATE()),
    foto_perfil_url  NVARCHAR(500)     NULL,
    CONSTRAINT PK_usuarios PRIMARY KEY (id),
    CONSTRAINT UQ_usuarios_email UNIQUE (email),
    CONSTRAINT CK_usuarios_rol CHECK (rol IN (N'VOLUNTARIO', N'ORGANIZACION', N'ADMIN')),
    CONSTRAINT CK_usuarios_estado CHECK (estado IN (N'ACTIVO', N'INACTIVO'))
);
GO

CREATE TABLE dbo.habilidades (
    id     INT IDENTITY(1,1) NOT NULL,
    nombre NVARCHAR(100)     NOT NULL,
    CONSTRAINT PK_habilidades PRIMARY KEY (id),
    CONSTRAINT UQ_habilidades_nombre UNIQUE (nombre)
);
GO

CREATE TABLE dbo.tipos_organizacion (
    id     INT IDENTITY(1,1) NOT NULL,
    nombre NVARCHAR(100)     NOT NULL,
    CONSTRAINT PK_tipos_organizacion PRIMARY KEY (id),
    CONSTRAINT UQ_tipos_organizacion_nombre UNIQUE (nombre)
);
GO

CREATE TABLE dbo.distritos (
    id     INT IDENTITY(1,1) NOT NULL,
    nombre NVARCHAR(100)     NOT NULL,
    activo BIT              NOT NULL DEFAULT(1),
    CONSTRAINT PK_distritos PRIMARY KEY (id),
    CONSTRAINT UQ_distritos_nombre UNIQUE (nombre)
);
GO

CREATE TABLE dbo.categorias_evento (
    id     INT IDENTITY(1,1) NOT NULL,
    nombre NVARCHAR(255)     NOT NULL,
    CONSTRAINT PK_categorias_evento PRIMARY KEY (id),
    CONSTRAINT UQ_categorias_evento_nombre UNIQUE (nombre)
);
GO

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
    imagen_principal NVARCHAR(500) NULL,
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

CREATE INDEX IX_eventos_filtros
ON dbo.eventos (distrito, categoria_id, fecha_evento, estado);
GO

CREATE INDEX IX_inscripciones_evento
ON dbo.evento_inscripciones (evento_id, estado);
GO

INSERT INTO dbo.categorias_evento(nombre) VALUES
(N'Niñez'),
(N'Adulto mayor'),
(N'Animales'),
(N'Salud'),
(N'Medio ambiente');
GO

INSERT INTO dbo.habilidades(nombre) VALUES
(N'Organización'),
(N'Comunicación'),
(N'Primeros auxilios'),
(N'Cocina'),
(N'Limpieza'),
(N'Logística'),
(N'Fotografía'),
(N'Diseño');
GO

INSERT INTO dbo.tipos_organizacion(nombre) VALUES
(N'ONG'),
(N'Asociación'),
(N'Comunidad'),
(N'Iglesia / Comunidad religiosa'),
(N'Municipalidad / Entidad pública'),
(N'Universidad / Instituto'),
(N'Empresa (RSE)'),
(N'Fundación'),
(N'Colectivo'),
(N'Otro');
GO

INSERT INTO dbo.distritos(nombre) VALUES
(N'Ancón'),
(N'Ate'),
(N'Barranco'),
(N'Breña'),
(N'Carabayllo'),
(N'Chaclacayo'),
(N'Chorrillos'),
(N'Cieneguilla'),
(N'Comas'),
(N'El Agustino'),
(N'Independencia'),
(N'Jesús María'),
(N'La Molina'),
(N'La Victoria'),
(N'Lima'),
(N'Lince'),
(N'Los Olivos'),
(N'Lurigancho'),
(N'Lurín'),
(N'Magdalena del Mar'),
(N'Miraflores'),
(N'Pachacámac'),
(N'Pucusana'),
(N'Pueblo Libre'),
(N'Puente Piedra'),
(N'Punta Hermosa'),
(N'Punta Negra'),
(N'Rímac'),
(N'San Bartolo'),
(N'San Borja'),
(N'San Isidro'),
(N'San Juan de Lurigancho'),
(N'San Juan de Miraflores'),
(N'San Luis'),
(N'San Martín de Porres'),
(N'San Miguel'),
(N'Santa Anita'),
(N'Santa María del Mar'),
(N'Santa Rosa'),
(N'Santiago de Surco'),
(N'Surquillo'),
(N'Villa El Salvador'),
(N'Villa María del Triunfo');
GO

INSERT INTO dbo.usuarios(email, password_hash, rol, es_mayor_edad, estado, foto_perfil_url)
VALUES 
(N'org@demo.com',   N'ORG123',   N'ORGANIZACION', 1, N'ACTIVO', N'https://picsum.photos/300/300?random=10'),
(N'vol@demo.com',   N'VOL123',   N'VOLUNTARIO',   1, N'ACTIVO', NULL),
(N'admin@racoca.com', N'ADMIN123', N'ADMIN',      1, N'ACTIVO', NULL);
GO

DECLARE @OrgId INT = (SELECT TOP 1 id FROM dbo.usuarios WHERE email = N'org@demo.com');

INSERT INTO dbo.organizacion_perfil(
    usuario_id,
    nombres_responsable,
    apellidos_responsable,
    celular_responsable,
    nombre_organizacion,
    tipo_organizacion,
    descripcion_corta,
    ciudad,
    distrito,
    direccion
)
VALUES (
    @OrgId,
    N'Ana',
    N'Pérez',
    N'999999999',
    N'ONG Demo',
    N'ONG',
    N'Ayuda social en Lima',
    N'Lima',
    N'Miraflores',
    N'Av. Demo 123'
);
GO

DECLARE @VolId INT = (SELECT TOP 1 id FROM dbo.usuarios WHERE email = N'vol@demo.com');

INSERT INTO dbo.voluntario_perfil(
    usuario_id,
    nombres,
    apellidos,
    sexo,
    celular,
    fecha_nacimiento,
    ciudad,
    distrito
)
VALUES (
    @VolId,
    N'Luis',
    N'Gómez',
    N'M',
    N'988888888',
    '2000-01-01',
    N'Lima',
    N'San Miguel'
);
GO

DECLARE @OrgId INT = (SELECT TOP 1 id FROM dbo.usuarios WHERE rol='ORGANIZACION');
DECLARE @CatSalud INT = (SELECT TOP 1 id FROM dbo.categorias_evento WHERE nombre = N'Salud');
DECLARE @CatNinez INT = (SELECT TOP 1 id FROM dbo.categorias_evento WHERE nombre = N'Niñez');
DECLARE @CatMedio INT = (SELECT TOP 1 id FROM dbo.categorias_evento WHERE nombre = N'Medio ambiente');

INSERT INTO dbo.eventos (
    organizacion_id,
    categoria_id,
    nombre,
    descripcion_corta,
    descripcion_larga,
    fecha_evento,
    hora_evento,
    ciudad,
    distrito,
    direccion,
    mapa_url,
    ilimitado,
    cupos_limite,
    requisitos,
    incluye,
    no_incluye,
    metodo_contacto,
    whatsapp_numero,
    imagen_principal,
    estado
)
VALUES
(@OrgId, @CatSalud,
 N'Campaña de salud preventiva',
 N'Apoyo en registro, orden y orientación a familias.',
 N'Jornada con charlas preventivas, apoyo en logística y distribución de material informativo.',
 '2026-01-22','09:00',
 N'Lima',N'Miraflores',N'Parque Kennedy',NULL,
 0,30,
 N'Puntualidad y actitud de servicio',
 N'Constancia digital',
 N'Atención médica especializada',
 N'WHATSAPP',N'987654321',
 N'/uploads/eventos/salud.jpg',N'PUBLICADO'),

(@OrgId, @CatNinez,
 N'Refuerzo escolar para niños',
 N'Acompañamiento educativo a niños de primaria.',
 N'Dinámicas de lectura, ejercicios de matemática básica y apoyo personalizado.',
 '2026-01-27','16:00',
 N'Lima',N'Los Olivos',N'Casa de la Juventud Los Olivos',NULL,
 0,20,
 N'Paciencia y compromiso',
 N'Material impreso',
 N'Certificado físico',
 N'WHATSAPP',N'987654321',
 N'/uploads/eventos/educacion.jpg',N'PUBLICADO'),

(@OrgId, @CatMedio,
 N'Limpieza de parque y reciclaje',
 N'Recolección y segregación de residuos.',
 N'Actividad de limpieza con puntos de acopio y orientación sobre reciclaje.',
 '2026-02-03','08:30',
 N'Lima',N'Barranco',N'Parque Municipal - Barranco',NULL,
 1,NULL,
 N'Bloqueador y gorra',
 N'Bolsas y guantes (si tienes)',
 N'Bebidas alcohólicas',
 N'WHATSAPP',N'987654321',
 N'/uploads/eventos/medioambiente.jpg',N'PUBLICADO');
GO

CREATE OR ALTER PROCEDURE dbo.sp_eventos_contar
(
    @buscar NVARCHAR(100) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT COUNT(*) AS Total
    FROM dbo.eventos e
    WHERE e.estado = N'PUBLICADO'
      AND (@buscar IS NULL OR e.nombre LIKE N'%' + @buscar + N'%');
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_eventos_paginados
(
    @pagina   INT,
    @pageSize INT,
    @buscar   NVARCHAR(100) = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        e.id,
        e.nombre,
        e.descripcion_corta,
        e.fecha_evento,
        e.hora_evento,
        e.distrito,
        e.ciudad,
        e.imagen_principal,
        e.estado
    FROM dbo.eventos e
    WHERE e.estado = N'PUBLICADO'
      AND (@buscar IS NULL OR e.nombre LIKE N'%' + @buscar + N'%')
    ORDER BY e.fecha_evento, e.hora_evento
    OFFSET (@pagina - 1) * @pageSize ROWS
    FETCH NEXT @pageSize ROWS ONLY;
END;
GO

CREATE OR ALTER PROCEDURE dbo.sp_eventos_listar_por_organizacion
    @organizacionId INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        e.id,
        e.nombre,
        e.descripcion_corta,
        e.fecha_evento,
        e.hora_evento,
        e.ciudad,
        e.distrito,
        e.estado,
        e.imagen_principal,
        c.nombre AS categoria_nombre
    FROM dbo.eventos e
    INNER JOIN dbo.categorias_evento c ON c.id = e.categoria_id
    WHERE e.organizacion_id = @organizacionId
    ORDER BY e.fecha_evento DESC, e.hora_evento DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_eventos_detalle
    @id INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        e.*,
        c.nombre AS categoria_nombre,
        op.nombre_organizacion
    FROM dbo.eventos e
    INNER JOIN dbo.categorias_evento   c  ON c.id = e.categoria_id
    INNER JOIN dbo.organizacion_perfil op ON op.usuario_id = e.organizacion_id
    WHERE e.id = @id;
END
GO


CREATE OR ALTER PROCEDURE dbo.sp_eventos_inscribir_voluntario
    @eventoId     INT,
    @voluntarioId INT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (
        SELECT 1 
        FROM dbo.evento_inscripciones 
        WHERE evento_id = @eventoId 
          AND voluntario_id = @voluntarioId
    )
    BEGIN
        INSERT INTO dbo.evento_inscripciones (evento_id, voluntario_id)
        VALUES (@eventoId, @voluntarioId);
    END

    SELECT id, evento_id, voluntario_id, fecha, estado
    FROM dbo.evento_inscripciones
    WHERE evento_id = @eventoId AND voluntario_id = @voluntarioId;
END
GO
