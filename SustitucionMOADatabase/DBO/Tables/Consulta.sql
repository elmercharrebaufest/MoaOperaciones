CREATE TABLE [dbo].[Consulta]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[CodigoCorredor] varchar(max) NULL,
	[RazonSocialCorredor] varchar(max) NULL,
	[CodigoProveedor] varchar(max) NOT NULL,
	[RazonSocialProveedor] varchar(max) NULL,
	[Categoria_Id] INT NOT NULL,
	[SubCategoria_Id] INT NULL,
	[Asunto] [nvarchar](max) NOT NULL,
	[EstadoConsulta_Id] INT NOT NULL,
	[Usuario_Id] INT NOT NULL,
	[FechaCreacion] DATETIME2 NOT NULL DEFAULT (getdate()), 
	[FechaUltimaModificacion] DATETIME2 NOT NULL DEFAULT (getdate()), 
[UsuarioInterno_Id] INT NULL, 
    [FechaVtoReapertura] DATETIME2 NULL, 
    CONSTRAINT [PK_dbo.Consulta] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Consulta]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Consulta_dbo.Categoria_Categoria_Id] FOREIGN KEY([Categoria_Id])
REFERENCES [dbo].[Categoria] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Consulta] CHECK CONSTRAINT [FK_dbo.Consulta_dbo.Categoria_Categoria_Id]
GO

ALTER TABLE [dbo].[Consulta]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Consulta_dbo.SubCategoria_SubCategoria_Id] FOREIGN KEY([SubCategoria_Id])
REFERENCES [dbo].[SubCategoria] ([Id])
ON DELETE NO ACTION
GO

ALTER TABLE [dbo].[Consulta] CHECK CONSTRAINT [FK_dbo.Consulta_dbo.SubCategoria_SubCategoria_Id]
GO

ALTER TABLE [dbo].[Consulta]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Consulta_dbo.EstadoConsulta_EstadoConsulta_Id] FOREIGN KEY([EstadoConsulta_Id])
REFERENCES [dbo].[EstadoConsulta] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Consulta] CHECK CONSTRAINT [FK_dbo.Consulta_dbo.EstadoConsulta_EstadoConsulta_Id]
GO

ALTER TABLE [dbo].[Consulta]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Consulta_dbo.Usuario_Usuario_Id] FOREIGN KEY([Usuario_Id])
REFERENCES [dbo].[Usuario] ([Id])
ON DELETE NO ACTION
GO

ALTER TABLE [dbo].[Consulta] CHECK CONSTRAINT [FK_dbo.Consulta_dbo.Usuario_Usuario_Id]
GO
