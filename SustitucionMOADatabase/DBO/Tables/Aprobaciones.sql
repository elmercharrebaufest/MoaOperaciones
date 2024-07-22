CREATE TABLE [dbo].[Aprobaciones](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Ingresante_CDS] [nvarchar](100) NULL,
    [NRO_ES_LOCAL] [nvarchar](20) NULL,
	[NRO_ES_SAP] [int] NULL,
	[Fecha_Documento] [date] NULL,
	[Fecha_Contabilizacion] [date] NULL,
	[Referencia] [nvarchar](50) NULL,
	[Cantidad] [float] NULL,
	[Descripcion_ES] [nvarchar](max) NULL,
	[Importe] [float] NULL,
	[Estado_certificacion] [nvarchar](30) NULL,
	[Aprobada_automaticamente] [bit] NULL,
	[Aprobador_CDS] [nvarchar](100) NULL,
	[Fiscal_SOLPED] [nvarchar](100) NULL,
	[Suplente] [nvarchar](100) NULL,
	[Area_Fiscal] [nvarchar](100) NULL,
	[Fecha_Carga_ES] [date] NULL,
	[Fecha_aprobacion] [date] NULL,
	[Fecha_rechazo] [date] NULL,
	[Motivo_rechazo] [nvarchar](max) NULL,
	[Notificaciones_enviadas] [bit] NULL,
	[NRO_OC] [nvarchar](10) NULL,
	[NRO_POS] [nvarchar](10) NULL,
	[Nro_linea] [nvarchar](10) NULL,
	[Nro_servicio] [nvarchar](10) NULL,
	[Texto_breve_servicio] [nvarchar](max) NULL,
	[UM] [nvarchar](10) NULL,
	[Monto] [float] NULL,
	[Cantidad_Anterior] [float] NULL,
	[Cantidad_a_certificar] [nvarchar](10) NULL,
	[Porcentaje_a_certificar] [nchar](10) NULL,
	[Monto_a_certificar] [float] NULL,
	[Monto_total] [float] NULL,
	[Planned_package] NVARCHAR(50) NULL, 
    [Planned_line] NVARCHAR(50) NULL, 
	[Proveedor] NVARCHAR(50) NULL,
    CONSTRAINT [PK_Aprobaciones] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO



