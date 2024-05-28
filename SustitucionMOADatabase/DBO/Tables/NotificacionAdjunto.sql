CREATE TABLE [dbo].[NotificacionAdjunto] (
    AdjuntoId INT PRIMARY KEY IDENTITY(1,1), 
    NotificacionId INT,
    AdjuntoTipo NVARCHAR(50),
    AdjuntoNombre NVARCHAR(100),
    AdjuntoContenido NVARCHAR(MAX)
);
