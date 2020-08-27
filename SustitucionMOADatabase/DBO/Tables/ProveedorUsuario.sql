CREATE TABLE [dbo].[ProveedorUsuario](
	[Proveedor_Id] [int] NOT NULL,
	[Usuario_Id] [int] NOT NULL,
 CONSTRAINT [PK_dbo.ProveedorUsuario] PRIMARY KEY CLUSTERED 
(
	[Proveedor_Id] ASC,
	[Usuario_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ProveedorUsuario]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ProveedorUsuario_dbo.Proveedor_Proveedor_Id] FOREIGN KEY([Proveedor_Id])
REFERENCES [dbo].[Proveedor] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ProveedorUsuario] CHECK CONSTRAINT [FK_dbo.ProveedorUsuario_dbo.Proveedor_Proveedor_Id]
GO

ALTER TABLE [dbo].[ProveedorUsuario]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ProveedorUsuario_dbo.Usuario_Usuario_Id] FOREIGN KEY([Usuario_Id])
REFERENCES [dbo].[Usuario] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ProveedorUsuario] CHECK CONSTRAINT [FK_dbo.ProveedorUsuario_dbo.Usuario_Usuario_Id]
GO

