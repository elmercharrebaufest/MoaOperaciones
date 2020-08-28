CREATE TABLE [dbo].[Localidad](
	[LocalidadId] [int] IDENTITY(1,1) NOT NULL,
	[CodLocalidad] [int] NOT NULL,
	[Nombre] [nvarchar](max) NULL,
	[ProvinciaId] [int] NOT NULL,
	[PartidoId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.Localidad] PRIMARY KEY CLUSTERED 
(
	[LocalidadId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Localidad]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Localidad_dbo.Partido_PartidoId] FOREIGN KEY([PartidoId])
REFERENCES [dbo].[Partido] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Localidad] CHECK CONSTRAINT [FK_dbo.Localidad_dbo.Partido_PartidoId]
GO

ALTER TABLE [dbo].[Localidad]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Localidad_dbo.Provincia_ProvinciaId] FOREIGN KEY([ProvinciaId])
REFERENCES [dbo].[Provincia] ([ProvinciaId])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Localidad] CHECK CONSTRAINT [FK_dbo.Localidad_dbo.Provincia_ProvinciaId]
GO

