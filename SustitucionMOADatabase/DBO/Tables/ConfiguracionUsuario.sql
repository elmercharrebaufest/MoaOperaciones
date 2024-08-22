CREATE TABLE [dbo].[ConfiguracionUsuario]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [Tipo] INT NOT NULL, 
    [Usuario_Id] INT NOT NULL, 
    [Valor] NVARCHAR(500) NOT NULL,
    CONSTRAINT [FK_ConfiguracionUsuario_Usuario] FOREIGN KEY (Usuario_Id) REFERENCES Usuario(Id),
)
