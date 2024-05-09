CREATE TABLE [dbo].[Curso]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[Nombre] NCHAR(255) NOT NULL, 
	[FechaCreacion] DATETIME  DEFAULT (getdate()) NOT NULL,
	[Acceso] NCHAR(255) NOT NULL,
)
