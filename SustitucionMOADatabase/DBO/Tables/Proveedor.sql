CREATE TABLE [dbo].[Proveedor](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CUIT] [nvarchar](max) NULL,
	[RazonSocial] [nvarchar](max) NULL,
	[CodigoProveedor] [nvarchar](max) NULL,
	[Mail] [nvarchar](max) NULL,
	[EstadoAprobacion] [int] NOT NULL,
	[Observaciones] [nvarchar](max) NULL,
	[IdDataAgro] [int] NULL,
	[IdComercialDataAgro] [int] NULL,
 [EstadoSIPER] VARCHAR(MAX) NULL, 
 	 [VinculoConEmpleadosDeMolinos] BIT NULL, 
    [VinculoConFuncionariosPublicos] BIT NULL, 
    [FechaSolicitud] DATETIME NULL, 
    [Comercial] VARCHAR(200) NULL, 
    [IdProveedorCorredor] INT NULL, 
    [Telefono] VARCHAR(MAX) NULL, 
    [RealizarAnalisisNOSIS] BIT NULL, 
    [IdRubro] INT NULL, 
    [CondicionDePago] VARCHAR(150) NULL, 
    [ServicioPrestado] VARCHAR(150) NULL, 
    [OrganizacionDeCompra] VARCHAR(150) NULL, 
    [RazonDeEleccion] VARCHAR(150) NULL, 
    [FacturacionAnual] BIGINT NULL, 
    [SolicitanteInterno] VARCHAR(150) NULL, 
    [RequiereVerificacionCompras] BIT NULL, 
    [IdSituacionIVA] INT NULL, 
    [IdIngresoBruto] INT NULL, 
    [CBU] VARCHAR(50) NULL, 
    [IngresoAPlanta] BIT NULL, 
    [AltaInterna] BIT NULL, 
    [TipoProveedor_Id] INT NULL, 
    [SiperObligatorio] BIT NULL, 
   
    [ContieneDocumentacionFisica] BIT NULL, 
    [IdSolicitanteInternoAltaGranos] INT NULL, 
    [EstadoSISA] VARCHAR(150) NULL, 
    [EsRevendedor] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_dbo.Proveedor] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY], 
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Proveedor]  WITH CHECK ADD  CONSTRAINT [FK_Proveedor_ProveedorCorredor] FOREIGN KEY([IdProveedorCorredor])
REFERENCES [dbo].[Proveedor] ([Id])
GO

ALTER TABLE [dbo].[Proveedor] CHECK CONSTRAINT [FK_Proveedor_ProveedorCorredor]
GO

ALTER TABLE [dbo].[Proveedor]  WITH CHECK ADD  CONSTRAINT [FK_Proveedor_SolicitanteInternoAltaGranos] FOREIGN KEY([IdSolicitanteInternoAltaGranos])
REFERENCES [dbo].[Usuario] ([Id])
GO

ALTER TABLE [dbo].[Proveedor] CHECK CONSTRAINT [FK_Proveedor_SolicitanteInternoAltaGranos]
GO

ALTER TABLE [dbo].[Proveedor]  WITH CHECK ADD  CONSTRAINT [FK_Proveedor_Rubro] FOREIGN KEY([IdRubro])
REFERENCES [dbo].[Rubro] ([Id])
GO
ALTER TABLE [dbo].[Proveedor] CHECK CONSTRAINT [FK_Proveedor_Rubro]
GO

ALTER TABLE [dbo].[Proveedor] CHECK CONSTRAINT [FK_Proveedor_ProveedorCorredor]
GO

ALTER TABLE [dbo].[Proveedor]  WITH CHECK ADD  CONSTRAINT [FK_Proveedor_SituacionIVA] FOREIGN KEY([IdSituacionIVA])
REFERENCES [dbo].[SituacionIVA] ([Id])
GO
ALTER TABLE [dbo].[Proveedor] CHECK CONSTRAINT [FK_Proveedor_SituacionIVA]
GO

ALTER TABLE [dbo].[Proveedor]  WITH CHECK ADD  CONSTRAINT [FK_Proveedor_IngresoBruto] FOREIGN KEY([IdIngresoBruto])
REFERENCES [dbo].[IngresoBruto] ([Id])
GO
ALTER TABLE [dbo].[Proveedor] CHECK CONSTRAINT [FK_Proveedor_IngresoBruto]
GO