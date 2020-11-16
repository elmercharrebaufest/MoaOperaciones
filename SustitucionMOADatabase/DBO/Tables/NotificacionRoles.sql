CREATE TABLE [dbo].[RolesNotificacion]
(
	[Notificacion_Id] INT NOT NULL , 
    [Rol_Id] INT NOT NULL, 
    PRIMARY KEY ([Rol_Id], [Notificacion_Id]), 
    CONSTRAINT [FK_RolesNotificacion_Notificacion] FOREIGN KEY (Notificacion_Id) REFERENCES Notificacion(Id), 
    CONSTRAINT [FK_RolesNotificacion_Rol] FOREIGN KEY (Rol_Id) REFERENCES Rol(Id)
)
