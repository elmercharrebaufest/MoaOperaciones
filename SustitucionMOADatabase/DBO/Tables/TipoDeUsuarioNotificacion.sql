CREATE TABLE [dbo].[TipoDeUsuarioNotificacion]
(
	[Notificacion_Id] INT NOT NULL , 
    [TipoUsuario_Id] INT NOT NULL, 
    PRIMARY KEY ([TipoUsuario_Id], [Notificacion_Id]),
    CONSTRAINT [FK_TipoDeUsuarioNotificacion_Notificacion] FOREIGN KEY (Notificacion_Id) REFERENCES Notificacion(Id), 
    CONSTRAINT [FK_TipoDeUsuarioNotificacion_TipoDeUsuario] FOREIGN KEY (TipoUsuario_Id) REFERENCES TipoUsuario(Id)
)
