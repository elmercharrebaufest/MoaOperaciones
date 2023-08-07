CREATE TABLE [dbo].[UnidadMedidaSap]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY (1, 1), 
	[UM] varchar(500) NOT NULL, 
	[Comercial] varchar(500) NOT NULL, 
	[Tecnica] varchar(500) NOT NULL, 
	[TextoUM] varchar(500) NOT NULL, 
	[TextoUM2] varchar(500) NOT NULL, 
)
