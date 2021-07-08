CREATE TABLE [dbo].[Comentario]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Detalle] [nvarchar](max) NOT NULL,
	[Fecha] DATETIME NOT NULL DEFAULT (getdate()),
	[Consulta_Id] INT NOT NULL,
	[Usuario_Id] INT NOT NULL,
[Recordado] BIT NULL DEFAULT 0, 
    [FechaRecordado] DATETIME NULL DEFAULT (getdate()), 
    CONSTRAINT [PK_dbo.Comentario] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Comentario]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Comentario_dbo.Usuario_Usuario_Id] FOREIGN KEY([Usuario_Id])
REFERENCES [dbo].[Usuario] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Comentario] CHECK CONSTRAINT [FK_dbo.Comentario_dbo.Usuario_Usuario_Id]
GO

ALTER TABLE [dbo].[Comentario]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Comentario_dbo.Consulta_Consulta_Id] FOREIGN KEY([Consulta_Id])
REFERENCES [dbo].[Consulta] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Comentario] CHECK CONSTRAINT [FK_dbo.Comentario_dbo.Consulta_Consulta_Id]
GO
