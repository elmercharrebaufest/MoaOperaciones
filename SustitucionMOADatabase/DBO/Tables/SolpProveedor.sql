CREATE TABLE [dbo].[SolpProveedor]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SolpPosicion_Id] [int] NOT NULL,
	[Proveedor_Id] [int] NULL,
	[RazonSocial] [nvarchar](max) NULL,
	[TipoFiltroProveedorSolp_Id] [int] NOT NULL,

CONSTRAINT [PK_dbo.SolpProveedor] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	CONSTRAINT [FK_SolpProveedor_SolpPosicion] FOREIGN KEY (SolpPosicion_Id) REFERENCES [SolpPosicion]([Id]) ON DELETE CASCADE,
	CONSTRAINT [FK_SolpProveedor_Proveedor] FOREIGN KEY (Proveedor_Id) REFERENCES [Proveedor]([Id]),
	CONSTRAINT [FK_SolpProveedor_TablaGeneral_TipoFiltroProveedorSolp] FOREIGN KEY (TipoFiltroProveedorSolp_Id) REFERENCES [TablaGeneral]([Id]),
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
