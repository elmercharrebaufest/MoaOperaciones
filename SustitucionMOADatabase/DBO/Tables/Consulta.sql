CREATE TABLE [dbo].[Consulta]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[Proveedor_Id] INT NOT NULL,
	[Nombre] [nvarchar](max) NOT NULL,
	[Email] [nvarchar](max) NOT NULL,
	[Telefono] [nvarchar](max) NULL,
	[Categoria_Id] INT NOT NULL,
	[Asunto] [nvarchar](max) NOT NULL,
	[EstadoConsulta_Id] INT NOT NULL,
	[FechaCreacion] DATETIME NOT NULL DEFAULT (getdate()), 
	[FechaUltimaModificacion] DATETIME NOT NULL DEFAULT (getdate()), 
	[RazonSocial] [nvarchar](max) NULL,
	[NombreVendedor] [nvarchar](max) NULL,
	[Contrato] [nvarchar](max) NULL,
	[CUIT] [nvarchar](max) NULL,
	[Comprobante] [nvarchar](max) NULL,
	[FechaPago] DATETIME NULL, 
	[Importe] NUMERIC NULL, 
	[Impuesto] NUMERIC NULL, 
	[Inscripcion] [nvarchar](max) NULL,
	[Motivo] [nvarchar](max) NULL,
CONSTRAINT [PK_dbo.Consulta] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Consulta]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Consulta_dbo.Proveedor_Proveedor_Id] FOREIGN KEY([Proveedor_Id])
REFERENCES [dbo].[Proveedor] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Consulta] CHECK CONSTRAINT [FK_dbo.Consulta_dbo.Proveedor_Proveedor_Id]
GO

ALTER TABLE [dbo].[Consulta]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Consulta_dbo.Categoria_Categoria_Id] FOREIGN KEY([Categoria_Id])
REFERENCES [dbo].[Categoria] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Categoria] CHECK CONSTRAINT [FK_dbo.Consulta_dbo.Categoria_Categoria_Id]
GO

ALTER TABLE [dbo].[Consulta]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Consulta_dbo.EstadoConsulta_EstadoConsulta_Id] FOREIGN KEY([EstadoConsulta_Id])
REFERENCES [dbo].[EstadoConsulta] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[EstadoConsulta] CHECK CONSTRAINT [FK_dbo.Consulta_dbo.EstadoConsulta_EstadoConsulta_Id]
GO

