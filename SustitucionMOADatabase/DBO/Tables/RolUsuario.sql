CREATE TABLE [dbo].[RolUsuario](
	[Rol_Id] [int] NOT NULL,
	[Usuario_Id] [int] NOT NULL,
 CONSTRAINT [PK_dbo.RolUsuario] PRIMARY KEY CLUSTERED 
(
	[Rol_Id] ASC,
	[Usuario_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[RolUsuario]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RolUsuario_dbo.Rol_Rol_Id] FOREIGN KEY([Rol_Id])
REFERENCES [dbo].[Rol] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[RolUsuario] CHECK CONSTRAINT [FK_dbo.RolUsuario_dbo.Rol_Rol_Id]
GO

ALTER TABLE [dbo].[RolUsuario]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RolUsuario_dbo.Usuario_Usuario_Id] FOREIGN KEY([Usuario_Id])
REFERENCES [dbo].[Usuario] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[RolUsuario] CHECK CONSTRAINT [FK_dbo.RolUsuario_dbo.Usuario_Usuario_Id]
GO

