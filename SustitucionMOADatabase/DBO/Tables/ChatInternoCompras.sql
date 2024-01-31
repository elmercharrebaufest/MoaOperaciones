CREATE TABLE [dbo].ChatInternoCompras(
    [Id] INT NOT NULL IDENTITY, 
	[Usuario_Id] [int] NULL, 
	[Solp_Id] [int] NULL, 
	[FechaEnvio] DATETIME2 (7) NULL,
	[Leido] bit  NULL,  
	[Mensaje] NVARCHAR(MAX) NULL,
    CONSTRAINT [FK.ChatInternoCompras_Usuario_UsuarioId] FOREIGN KEY ([Usuario_Id]) REFERENCES [Usuario]([Id]),
	CONSTRAINT [FK.ChatInternoCompras_Solp_SolpId] FOREIGN KEY ([Solp_Id]) REFERENCES [Solp]([Id]),
    CONSTRAINT [PK_ChatInternoCompras] PRIMARY KEY ([Id]),
);
