CREATE TABLE [dbo].ChatExternoCompras(
    [Id] INT NOT NULL IDENTITY, 
	[PeticionDeOferta_Id] [int] NULL, 
	[Usuario_Id] [int] NULL, 
	[FechaEnvio] DATETIME2 (7) NULL,
	[Leido] bit  NULL,  
	[Mensaje] NVARCHAR(MAX) NULL,
	[PeticionDeOfertaUsuario_Id] [int] NULL, 
    CONSTRAINT [FK.ChatExternoCompras_Usuario_UsuarioId] FOREIGN KEY ([Usuario_Id]) REFERENCES [Usuario]([Id]),
	CONSTRAINT [FK.ChatExternoCompras_PeticionDeOfertaUsuario_PeticionDeOfertaUsuarioId] FOREIGN KEY ([PeticionDeOfertaUsuario_Id]) REFERENCES [PeticionDeOfertaUsuario]([Id]),
    CONSTRAINT [PK_ChatExternoCompras] PRIMARY KEY ([Id]),
);
