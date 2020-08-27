CREATE TABLE [dbo].[RolPermisoPorRol](
	[Rol_Id] [int] NOT NULL,
	[PermisoPorRol_Id] [int] NOT NULL,
 CONSTRAINT [PK_dbo.RolPermisoPorRol] PRIMARY KEY CLUSTERED 
(
	[Rol_Id] ASC,
	[PermisoPorRol_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[RolPermisoPorRol]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RolPermisoPorRol_dbo.PermisoPorRol_PermisoPorRol_Id] FOREIGN KEY([PermisoPorRol_Id])
REFERENCES [dbo].[PermisoPorRol] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[RolPermisoPorRol] CHECK CONSTRAINT [FK_dbo.RolPermisoPorRol_dbo.PermisoPorRol_PermisoPorRol_Id]
GO

ALTER TABLE [dbo].[RolPermisoPorRol]  WITH CHECK ADD  CONSTRAINT [FK_dbo.RolPermisoPorRol_dbo.Rol_Rol_Id] FOREIGN KEY([Rol_Id])
REFERENCES [dbo].[Rol] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[RolPermisoPorRol] CHECK CONSTRAINT [FK_dbo.RolPermisoPorRol_dbo.Rol_Rol_Id]
GO

