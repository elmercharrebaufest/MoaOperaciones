CREATE TABLE [dbo].[RelacionSolicitanteExternoJefeMoa]
(
	[Id] INT NOT NULL IDENTITY (1, 1), 
	[Usuario_Id] INT NOT NULL,
	[JefeMoa_Id] INT NOT NULL,
	CONSTRAINT [PK_RelacionSolicitanteExternoJefeMoa] PRIMARY KEY CLUSTERED ([Id] ASC),
	CONSTRAINT [FK_RelacionSolicitanteExternoJefeMoa_Usuario] FOREIGN KEY ([Usuario_Id]) REFERENCES [Usuario]([Id]),
	CONSTRAINT [FK_RelacionSolicitanteExternoJefeMoa_JefeMoa] FOREIGN KEY ([JefeMoa_Id]) REFERENCES [Usuario]([Id]),
)
