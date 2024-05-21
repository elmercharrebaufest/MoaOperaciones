CREATE TABLE dbo.NotificacionLeida
	(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
	Notificacion_Id INT NOT NULL,
	Usuario_Id      INT NOT NULL,
	[FechaLeida] DATETIME NULL
	)

