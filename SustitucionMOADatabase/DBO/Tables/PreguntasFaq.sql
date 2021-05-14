CREATE TABLE [dbo].[PreguntasFaq]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [IdCategoriaFaq] INT NOT NULL, 
    [Nombre] NCHAR(10) NOT NULL, 
    [Texto] NCHAR(10) NOT NULL
)
