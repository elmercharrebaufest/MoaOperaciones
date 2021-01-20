CREATE TABLE [dbo].[ComentarioArchivo](
	[Comentario_Id] [int] NOT NULL,
	[Archivo_Id] [int] NOT NULL,
 CONSTRAINT [PK_dbo.ComentarioArchivo] PRIMARY KEY CLUSTERED 
(
	[Comentario_Id] ASC,
	[Archivo_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ComentarioArchivo]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ComentarioArchivo_dbo.Comentario_Comentario_Id] FOREIGN KEY([Comentario_Id])
REFERENCES [dbo].[Comentario] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ComentarioArchivo] CHECK CONSTRAINT [FK_dbo.ComentarioArchivo_dbo.Comentario_Comentario_Id]
GO

ALTER TABLE [dbo].[ComentarioArchivo]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ProveedorUsuario_dbo.Archivo_Archivo_Id] FOREIGN KEY([Archivo_Id])
REFERENCES [dbo].[Archivo] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ComentarioArchivo] CHECK CONSTRAINT [FK_dbo.ProveedorUsuario_dbo.Archivo_Archivo_Id]
GO

