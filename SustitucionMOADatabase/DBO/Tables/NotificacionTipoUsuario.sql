
CREATE TABLE [dbo].[NotificacionTipoUsuario] (
    [Notificacion_Id] [int] NOT NULL,
    [TipoUsuario_Id] [int] NOT NULL,
    CONSTRAINT [PK_dbo.NotificacionTipoUsuario] PRIMARY KEY ([Notificacion_Id], [TipoUsuario_Id])
)