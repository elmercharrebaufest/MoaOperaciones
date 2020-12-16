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
    CONSTRAINT [PK_dbo.Proveedor] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Proveedor]  WITH CHECK ADD  CONSTRAINT [FK_Proveedor_ProveedorCorredor] FOREIGN KEY([IdProveedorCorredor])
REFERENCES [dbo].[Proveedor] ([Id])
GO

ALTER TABLE [dbo].[Proveedor] CHECK CONSTRAINT [FK_Proveedor_ProveedorCorredor]
GO
