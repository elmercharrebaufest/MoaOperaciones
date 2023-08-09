CREATE TABLE [dbo].[MaterialSolp](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Centro_Id] [int] NULL,
	[Codigo] [nvarchar](50) NULL,
	[CodigoSap] [nvarchar](50) NULL,
	[Descripcion] [nvarchar](200) NULL,
	[GrupoArticulo_Id] [int] NULL,
	[TipoMaterial] NVARCHAR(100) NULL,
	[UnidadMedidaBase_Id] [int] NULL,
	[UnidadMedidaCompras_Id] [int] NULL,
	[UnidadMedidaSalida_Id] [int] NULL,
	[TipoValoracion] [varchar](50) NULL,
	[PrecioMaterial] [decimal](18, 0) NULL,
	[GrupoCompras_Id] [int] NULL,
	[CuentaMayor_Id] [int] NULL,
	[Estado] [bit] NULL,
[TextoAmpliado] NVARCHAR(MAX) NULL, 
    PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY];

GO

ALTER TABLE [dbo].[MaterialSolp] ADD  DEFAULT ((1)) FOR [Estado];

GO

ALTER TABLE [dbo].[MaterialSolp]  WITH CHECK ADD  CONSTRAINT [FK_MaterialSolp_TablaSap_CentroLogistico] FOREIGN KEY([Centro_Id])
REFERENCES [dbo].[TablaSap] ([Id])
GO

ALTER TABLE [dbo].[MaterialSolp] CHECK CONSTRAINT [FK_MaterialSolp_TablaSap_CentroLogistico]

GO

ALTER TABLE [dbo].[MaterialSolp]  WITH CHECK ADD  CONSTRAINT [FK_MaterialSolp_TablaSap_GrupoArticulo] FOREIGN KEY([GrupoArticulo_Id])
REFERENCES [dbo].[TablaSap] ([Id])
GO

ALTER TABLE [dbo].[MaterialSolp] CHECK CONSTRAINT [FK_MaterialSolp_TablaSap_GrupoArticulo]

GO

ALTER TABLE [dbo].[MaterialSolp]  WITH CHECK ADD  CONSTRAINT [FK_MaterialSolp_TablaSap_UnidadMedidaBase] FOREIGN KEY([UnidadMedidaBase_Id])
REFERENCES [dbo].[TablaSap] ([Id])
GO

ALTER TABLE [dbo].[MaterialSolp] CHECK CONSTRAINT [FK_MaterialSolp_TablaSap_UnidadMedidaBase]

GO

ALTER TABLE [dbo].[MaterialSolp]  WITH CHECK ADD  CONSTRAINT [FK_MaterialSolp_TablaSap_UnidadMedidaCompras] FOREIGN KEY([UnidadMedidaCompras_Id])
REFERENCES [dbo].[TablaSap] ([Id])
GO

ALTER TABLE [dbo].[MaterialSolp] CHECK CONSTRAINT [FK_MaterialSolp_TablaSap_UnidadMedidaCompras]

GO

ALTER TABLE [dbo].[MaterialSolp]  WITH CHECK ADD  CONSTRAINT [FK_MaterialSolp_TablaSap_UnidadMedidaSalida] FOREIGN KEY([UnidadMedidaSalida_Id])
REFERENCES [dbo].[TablaSap] ([Id])
GO

ALTER TABLE [dbo].[MaterialSolp] CHECK CONSTRAINT [FK_MaterialSolp_TablaSap_UnidadMedidaSalida]

GO

ALTER TABLE [dbo].[MaterialSolp]  WITH CHECK ADD  CONSTRAINT [FK_MaterialSolp_TablaSap_GrupoCompras] FOREIGN KEY([GrupoCompras_Id])
REFERENCES [dbo].[TablaSap] ([Id])
GO

ALTER TABLE [dbo].[MaterialSolp] CHECK CONSTRAINT [FK_MaterialSolp_TablaSap_GrupoCompras]

GO

ALTER TABLE [dbo].[MaterialSolp]  WITH CHECK ADD  CONSTRAINT [FK_MaterialSolp_TablaSap_CuentaMayor] FOREIGN KEY([CuentaMayor_Id])
REFERENCES [dbo].[TablaSap] ([Id])
GO

ALTER TABLE [dbo].[MaterialSolp] CHECK CONSTRAINT [FK_MaterialSolp_TablaSap_CuentaMayor]

GO