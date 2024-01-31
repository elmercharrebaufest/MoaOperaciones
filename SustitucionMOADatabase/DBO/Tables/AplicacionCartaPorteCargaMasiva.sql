CREATE TABLE [dbo].[AplicacionCartaPorteCargaMasiva]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [UsuarioId] INT NOT NULL, 
    [Fecha] DATETIME NOT NULL, 
    [NombreArchivo] VARCHAR(100) NOT NULL, 
    CONSTRAINT [FK_AplicacionCartaPorteCargaMasiva_Usuario] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuario]([Id])
)
