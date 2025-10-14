CREATE TABLE [dbo].[SolpPosicion]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Codigo] [nvarchar](max) NULL,
	[Solp_Id] [int] NOT NULL,
	[FechaBaja] [datetime] NULL,
	[TipoPosicion_Id] [int] NULL,
	[TipoImputacion_Id] [int] NULL,
	[FechaEntregaServicio] [datetime] NULL,
	[FechaLiberacion] [datetime] NULL,
	[PlazoEntrega] [int] NULL,
	[Centro_Id] [int] NOT NULL,
	[Almacen_Id] [int] NULL,
	[NombreEntrega] [nvarchar](max) NULL,
	[CalleEntrega] [nvarchar](max) NULL,
	[NumeroEntrega] [nvarchar](max) NULL,
	[CpEntrega] [nvarchar](max) NULL,
	[PaisEntrega] [nvarchar](max) NULL,
	[GrupoCompras_Id] [int] NULL,
	[Solicitante] [nvarchar](max) NULL,
	[NroNecesidad] [nvarchar](max) NULL,
	[GrupoArticulo_Id] [int] NULL,
	[Moneda_Id] [int] NULL,
	[NumeroPedido] [nvarchar](max) NULL,
	[Estado] BIT NULL DEFAULT 1, 
	[Indice] INT NULL, 
	[TextoSuministro] NVARCHAR(MAX) NULL, 
	[Motivo] NVARCHAR(MAX) NULL, 
	[Modelo] NVARCHAR(MAX) NULL, 
	[ServicioSolp_Id] INT NULL, 
	[Tarea] NVARCHAR(MAX) NULL, 
	[Cantidad] DECIMAL(18, 2) NULL, 
	[Unidad_Id] INT NULL, 
	[PrecioBruto] DECIMAL(18, 2) NULL, 
	[CuentaMayor_Id] INT NULL, 
	[ValorTipoImputacion_Id] INT NULL, 
	[CantidadSubposicionesEnSAP] INT NULL , 
	[MaterialSolp_Id] INT NULL, 
	[EsConcluido] BIT NULL, 
	[ProvinciaId] INT NULL, 
	[NumeroContratoSuperior] NVARCHAR(50) NULL, 
	[NumeroPosicionContratoSuperior] NVARCHAR(50) NULL, 
	[NombreProveedor] NVARCHAR(MAX) NULL, 
	[ProveedorFijo] NVARCHAR(50) NULL, 
	[OrganizacionCompras] NVARCHAR(50) NULL, 
	[ProveedorAdjudicado_Id] INT NULL, 
	[RegistroInfoNro] NVARCHAR(50) NULL, 
	[OrganizacionDeComprasCodigo] NVARCHAR(50) NULL, 
	CONSTRAINT [PK_dbo.SolpPosicion] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	CONSTRAINT [FK_SolpPosicion_Solp] FOREIGN KEY (Solp_Id) REFERENCES [Solp]([Id]),
	CONSTRAINT [FK_SolpPosicion_TablaGeneral_TipoPosicion] FOREIGN KEY (TipoPosicion_Id) REFERENCES [TablaGeneral]([Id]),
	CONSTRAINT [FK_SolpPosicion_TablaGeneral_TipoImputacion] FOREIGN KEY (TipoImputacion_Id) REFERENCES [TablaGeneral]([Id]),
	CONSTRAINT [FK_SolpPosicion_TablaSap_Centro] FOREIGN KEY (Centro_Id) REFERENCES [TablaSap]([Id]),
	CONSTRAINT [FK_SolpPosicion_TablaSap_Almacen] FOREIGN KEY (Almacen_Id) REFERENCES [TablaSap]([Id]),
	CONSTRAINT [FK_SolpPosicion_TablaSap_GrupoCompras] FOREIGN KEY (GrupoCompras_Id) REFERENCES [TablaSap]([Id]),
	CONSTRAINT [FK_SolpPosicion_TablaSap_GrupoArticulo] FOREIGN KEY (GrupoArticulo_Id) REFERENCES [TablaSap]([Id]),
	CONSTRAINT [FK_SolpPosicion_TablaSap_Moneda] FOREIGN KEY (Moneda_Id) REFERENCES [TablaSap]([Id]),
	CONSTRAINT [FK_SolpPosicion_ServicioSolp] FOREIGN KEY (ServicioSolp_Id) REFERENCES [ServicioSolp]([Id]),
	CONSTRAINT [FK_SolpPosicion_MaterialSolp] FOREIGN KEY (MaterialSolp_Id) REFERENCES [MaterialSolp]([Id]),
	CONSTRAINT [FK_SolpPosicion_Usuario_ProveedorAdjudicado] FOREIGN KEY ([ProveedorAdjudicado_Id]) REFERENCES [dbo].[Usuario] ([Id])

) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

CREATE NONCLUSTERED INDEX [IX_SOLPPOSICION_SOLP] ON [dbo].[SolpPosicion]([Solp_Id])

GO

CREATE NONCLUSTERED INDEX [IX_SOLPPOSICION_GrupoCompras] ON [dbo].[SolpPosicion]([GrupoCompras_Id])

GO

CREATE NONCLUSTERED INDEX [IX_SOLPPOSICION_Centro] ON [dbo].[SolpPosicion]([Centro_Id])

GO

CREATE NONCLUSTERED INDEX [IX_SOLPPOSICION_TipoImputacion] ON [dbo].[SolpPosicion]([TipoImputacion_Id])

GO
