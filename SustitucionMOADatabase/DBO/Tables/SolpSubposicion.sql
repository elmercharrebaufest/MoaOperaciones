CREATE TABLE [dbo].[SolpSubposicion]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Codigo] [nvarchar](max) NULL,
	[SolpPosicion_Id] [int] NOT NULL,
	[Numero] [int] NOT NULL,
	[CodigoServicioSap_Id] [int] NULL,
	[ServicioSolp_Id] [int] NULL,
	[Tarea] [nvarchar](max) NULL,
	[CuentaMayor_Id] INT NULL,
	[Cantidad] [decimal](18, 2) NULL,
	[Unidad_Id] [int] NULL,
	[PrecioBruto] [decimal](18, 6) NULL,


[TipoImputacion_Id] INT NULL, 
    [Estado] BIT NULL DEFAULT 1, 
    CONSTRAINT [PK_dbo.SolpSubposicion] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
		CONSTRAINT [FK_SolpSubposicion_SolpPosicion] FOREIGN KEY (SolpPosicion_Id) REFERENCES [SolpPosicion]([Id]) ON DELETE CASCADE,
	CONSTRAINT [FK_SolpSubposicion_TablaSap_CodigoServicioSap] FOREIGN KEY (CodigoServicioSap_Id) REFERENCES [TablaSap]([Id]),
	CONSTRAINT [FK_SolpSubposicion_ServicioSolp] FOREIGN KEY (ServicioSolp_Id) REFERENCES [ServicioSolp]([Id]),
	CONSTRAINT [FK_SolpSubposicion_TablaSap_Unidad] FOREIGN KEY (Unidad_Id) REFERENCES [TablaSap]([Id]),
	CONSTRAINT [FK_SolpSubposicion_TablaSap_TipoImputacionSap] FOREIGN KEY (TipoImputacion_Id) REFERENCES [TablaSap]([Id]),
	CONSTRAINT [FK_SolpSubposicion_TablaSap_CuentaMayorSap] FOREIGN KEY (CuentaMayor_Id) REFERENCES [TablaSap]([Id]),
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

