
CREATE TABLE [dbo].[UsuarioGranos](
	[Id] [int] NOT NULL,
	[Comercial] [nvarchar](max) NULL,
	[RutaInformeComercialFirmado] [varchar](max) NULL,
	[RutaConstanciaCBU] [varchar](max) NULL,
	[RutaConstanciaCUIT] [varchar](max) NULL,
	[RutaInscripcionIIBB] [varchar](max) NULL,
	[RutaGananciasIVAIIBB] [varchar](max) NULL,
	[RutaSIPER] [varchar](max) NULL,
	[RutaDocumentacionEnBolsa] [varchar](max) NULL,
	[RutaConstanciaCBUMercaderia] [varchar](max) NULL,
	[RutaCertificadoExclusionIVA] [varchar](max) NULL,
	[RutaCertificadoExclusionIIBB] [varchar](max) NULL,
	[RutaCertificadoExclusionSUSS] [varchar](max) NULL,
	[RutaCertificadoExclusionGanancias] [varchar](max) NULL,
 CONSTRAINT [PK_dbo.UsuarioGranos] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[UsuarioGranos]  WITH CHECK ADD  CONSTRAINT [FK_dbo.UsuarioGranos_dbo.Usuario_Id] FOREIGN KEY([Id])
REFERENCES [dbo].[Usuario] ([Id])
GO

ALTER TABLE [dbo].[UsuarioGranos] CHECK CONSTRAINT [FK_dbo.UsuarioGranos_dbo.Usuario_Id]
GO
