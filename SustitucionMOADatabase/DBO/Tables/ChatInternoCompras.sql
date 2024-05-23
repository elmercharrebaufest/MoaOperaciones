CREATE TABLE [dbo].ChatInternoCompras(
    [Id] INT NOT NULL IDENTITY, 
	[Usuario_Id] [int] NULL, 
	[PeticionDeOferta_Id] [int] NULL, 
	[FechaEnvio] DATETIME2 (7) NULL,
	[Leido] bit  NULL,  
	[Mensaje] NVARCHAR(MAX) NULL,
    CONSTRAINT [FK.ChatInternoCompras_Usuario_UsuarioId] FOREIGN KEY ([Usuario_Id]) REFERENCES [Usuario]([Id]),
	CONSTRAINT [FK.ChatInternoCompras_Usuario_PeticionDeOfertaId] FOREIGN KEY ([PeticionDeOferta_Id]) REFERENCES [PeticionDeOferta]([Id]),
    CONSTRAINT [PK_ChatInternoCompras] PRIMARY KEY ([Id]),
);
