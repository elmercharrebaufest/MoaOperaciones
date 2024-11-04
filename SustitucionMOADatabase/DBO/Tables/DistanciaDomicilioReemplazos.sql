CREATE TABLE [dbo].[DistanciaDomicilioReemplazos]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[Anterior] VARCHAR(100) NOT NULL,
	[Nuevo] VARCHAR(100) NOT NULL
)
