CREATE TABLE [dbo].[SolpSubposicion]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SolpPosicion_Id] [int] NOT NULL,
	[Numero] [int] NOT NULL,
	[CodigoServicioSap_Id] [int] NULL,
	[Tarea] [nvarchar](max) NULL,
	[CuentaMayor] [nvarchar](max) NULL,
	[Cantidad] [decimal] NULL,
	[Unidad_Id] [int] NULL,
	[PrecioBruto] [decimal] NULL,

	[CentroCosto] [nvarchar](max) NULL,
	[OrdenOT] [nvarchar](max) NULL,
	[OrdenInversion] [nvarchar](max) NULL,
	[Siniestro] [nvarchar](max) NULL,

CONSTRAINT [PK_dbo.SolpSubposicion] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	CONSTRAINT [FK_SolpSubposicion_SolpPosicion] FOREIGN KEY (SolpPosicion_Id) REFERENCES [SolpPosicion]([Id]),
	CONSTRAINT [FK_SolpSubposicion_TablaSap_CodigoServicioSap] FOREIGN KEY (CodigoServicioSap_Id) REFERENCES [TablaSap]([Id]),
	CONSTRAINT [FK_SolpSubposicion_TablaSap_Unidad] FOREIGN KEY (Unidad_Id) REFERENCES [TablaSap]([Id]),

) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

