CREATE TABLE [dbo].[Notificacion]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [Nombre] VARCHAR(70) NULL, 
    [FechaInicio] DATETIME NULL, 
    [FechaFin] DATETIME NULL, 
    [Habilitada] BIT NULL, 
    [Borrada] BIT NULL, 
    [LinkAdjunto] VARCHAR(500) NULL, 
    [Mensaje] VARCHAR(MAX) NULL, 
    [Prioridad] INT NULL, 
    [FechaCreacion] DATETIME NULL
)
