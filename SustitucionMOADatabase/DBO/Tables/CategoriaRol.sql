CREATE TABLE [dbo].[CategoriaRol]
(
	[Rol_Id] INT NOT NULL , 
    [Categoria_Id] INT NOT NULL, 
    CONSTRAINT [PK_dbo.CategoriaRol] PRIMARY KEY CLUSTERED 
(
	[Rol_Id] ASC,
	[Categoria_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CategoriaRol]  WITH CHECK ADD  CONSTRAINT [FK_dbo.CategoriaRol_dbo.Rol_Rol_Id] FOREIGN KEY([Rol_Id])
REFERENCES [dbo].[Rol] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[CategoriaRol] CHECK CONSTRAINT [FK_dbo.CategoriaRol_dbo.Rol_Rol_Id]
GO

ALTER TABLE [dbo].[CategoriaRol]  WITH CHECK ADD  CONSTRAINT [FK_dbo.CategoriaRol_dbo.Categoria_Categoria_Id] FOREIGN KEY([Categoria_Id])
REFERENCES [dbo].[Categoria] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[CategoriaRol] CHECK CONSTRAINT [FK_dbo.CategoriaRol_dbo.Categoria_Categoria_Id]
GO
