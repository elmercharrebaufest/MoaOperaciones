CREATE TABLE [dbo].[PeticionDeOfertaVisualizacionPrecio] (
    [Id]                     INT            IDENTITY (1, 1) NOT NULL,
    [UsuarioCreador_Id] INT            NOT NULL,
    [PeticionDeOferta_Id] INT NOT NULL,
    [Archivo_Id] INT NULL,
    [FechaCreacion]          DATETIME2 (7)  NOT NULL,
    [Observaciones]              NVARCHAR(MAX)            NOT NULL,
    CONSTRAINT [PK_PeticionDeOfertaVisualizacionPrecio] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK.PeticionDeOfertaVisualizacionPrecio_Usuario_UsuarioCreadorId] FOREIGN KEY ([UsuarioCreador_Id]) REFERENCES [Usuario]([Id]),
    CONSTRAINT [FK.PeticionDeOfertaVisualizacionPrecio_Peticion_PeticionDeOferta_Id] FOREIGN KEY ([PeticionDeOferta_Id]) REFERENCES [PeticionDeOferta]([Id]),
    CONSTRAINT [FK.PeticionDeOfertaVisualizacionPrecio_Archivo_Archivo_Id] FOREIGN KEY ([Archivo_Id]) REFERENCES [Archivo]([Id]),

);


