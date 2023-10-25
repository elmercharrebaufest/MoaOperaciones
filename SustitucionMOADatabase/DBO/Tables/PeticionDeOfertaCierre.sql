CREATE TABLE [dbo].[PeticionDeOfertaCierre] (
    [Id]                     INT            IDENTITY (1, 1) NOT NULL,
    [PeticionDeOferta_Id] INT            NOT NULL,
    [Adjunto_Id] INT            NULL,
    [Usuario_Id] INT NOT NULL,
    [Fecha]          DATETIME2 (7)  NOT NULL,
    [Observacion]              NVARCHAR(1000)            NOT NULL,
    CONSTRAINT [PK_PeticionDeOfertaCierre] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK.PeticionDeOfertaCierre_Usuario_UsuarioId] FOREIGN KEY ([Usuario_Id]) REFERENCES [Usuario]([Id]),
    CONSTRAINT [FK.PeticionDeOfertaCierre_PeticionDeOferta_PeticionDeOfertaId] FOREIGN KEY([PeticionDeOferta_Id]) REFERENCES [PeticionDeOferta] ([Id]),

);


