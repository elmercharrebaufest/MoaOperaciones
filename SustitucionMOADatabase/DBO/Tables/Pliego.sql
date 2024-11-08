CREATE TABLE [dbo].[Pliego]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Codigo] [nvarchar](max) NULL,
	[NombreObra] [nvarchar](max) NULL,
	[FiscalContrato] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[Telefono] [nvarchar](max) NULL,
	[FechaHoraEntrega] [datetime2] NULL,
	[SupervisorSector] [nvarchar](max) NULL,
	[SupervisorTrabajo] [nvarchar](max) NULL,
	[TieneVisitaObraMasiva] [bit] NULL,
	[TieneObradores] [bit] NULL,
	[TieneMedioElevacion] [bit] NULL,
	[TieneTecnicoSeguridad] [bit] NULL,
	[TieneDescripcionTecnica] [bit] NULL,
	[TieneDocumentacionTecnica] [bit] NULL,
	[FechaHoraLimiteConsulta] [datetime2] NULL,
	[ObservacionesGeneracion] [nvarchar](max) NULL,
	[DiasEjecucion] [int] NULL,
	[ObservacionesCotizacion] [nvarchar](max) NULL,
	[JornadaLaboralDias] [nvarchar](max) NULL,
	[JornadaLaboralHorasDesde] DATETIMEOFFSET NULL,
	[JornadaLaboralHorasHasta] DATETIMEOFFSET NULL,
	[TieneAndamio] BIT NULL, 
	[TieneGrillaPersonal] BIT NULL, 
	[TieneFabricacionTallerExterno] BIT NULL, 
	[RevisadoPor] NVARCHAR(MAX) NULL, 
	[TieneCondicionesGenerales] BIT NULL,
	[RequisitoCiberseguridad] BIT NULL, 
	[ObservacionesCotizacionCondEsp] NVARCHAR(MAX) NULL, 
	CONSTRAINT [PK_dbo.Pliego] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
