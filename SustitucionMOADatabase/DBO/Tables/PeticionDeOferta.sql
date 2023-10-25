CREATE TABLE [dbo].[PeticionDeOferta] (
    [Id]                     INT            IDENTITY (1, 1) NOT NULL,
    [UsuarioCreador_Id] INT            NOT NULL,
    [Solp_Id] INT NOT NULL,
    [FechaCreacion]          DATETIME2 (7)  NOT NULL,
    [PlazoDeOferta]          DATETIME2 (7)  NOT NULL,
    [Observaciones]              NVARCHAR(MAX)            NOT NULL,   
    [RegistroInfo] BIT NULL, 
    [AdjuntoPliego] BIT NULL, 
    [UsuarioRevision_Id] INT NULL, 
    [FechaFinalizacionRevision] DATETIME2 NULL, 
    CONSTRAINT [PK_PeticionDeOferta] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK.PeticionDeOferta_Usuario_UsuarioCreadorId] FOREIGN KEY ([UsuarioCreador_Id]) REFERENCES [Usuario]([Id]),
    CONSTRAINT [FK.PeticionDeOfertaUsuario_Usuario_UsuarioRevision_Id] FOREIGN KEY ([UsuarioRevision_Id]) REFERENCES [Usuario]([Id]),
    CONSTRAINT [FK.PeticionDeOfertaUsuario_UsuarioCreador_Solp_Solp_Id] FOREIGN KEY ([Solp_Id]) REFERENCES [Solp]([Id])

);


