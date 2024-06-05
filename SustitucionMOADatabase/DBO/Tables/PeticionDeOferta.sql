CREATE TABLE [dbo].[PeticionDeOferta] (
    [Id]                     INT            IDENTITY (1, 1) NOT NULL,
    [UsuarioCreador_Id] INT            NOT NULL,
    [FechaCreacion]          DATETIME2 (7)  NOT NULL,
    [PlazoDeOferta]          DATETIME2 (7)  NOT NULL,
    [Observaciones]              NVARCHAR(MAX)            NOT NULL,   
    [RegistroInfo] BIT NULL, 
    [AdjuntoPliego] BIT NULL, 
    [RevisionTecnica_Id] INT NULL, 
    [Agrupada] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_PeticionDeOferta] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK.PeticionDeOferta_Usuario_UsuarioCreadorId] FOREIGN KEY ([UsuarioCreador_Id]) REFERENCES [Usuario]([Id]),
    CONSTRAINT [FK.PeticionDeOferta_RevisionTecnica_RevisionTecnica_Id] FOREIGN KEY ([RevisionTecnica_Id]) REFERENCES [PeticionDeOfertaRevisionTecnica]([Id]),


);


