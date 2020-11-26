CREATE TABLE [dbo].[NotificacionRol] (
    [Notificacion_Id] [int] NOT NULL,
    [Rol_Id] [int] NOT NULL,
    CONSTRAINT [PK_dbo.NotificacionRol] PRIMARY KEY ([Notificacion_Id], [Rol_Id])
)