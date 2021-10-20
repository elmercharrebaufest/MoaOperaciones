CREATE TABLE [dbo].[Solp]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UsuarioCreacion_Id] [int] NOT NULL,
	[UsuarioModificacion_Id] [int] NULL,
	[FechaCreacion] [datetime2] NOT NULL,
	[FechaModificacion] [datetime2] NULL,
	[Pliego_Id] [int] NULL,
	[ClaseDocumento_Id] [int] NULL,
	[NroSolp] [nvarchar](max) NULL,
	[EstadoSolpSap_Id] [int] NULL,
	[EstadoDocumento_Id] [int] NULL,
	[FechaBorrado] [datetime2] NULL, 
	[FechaCreacionSap] [datetime2] NULL,
	[FechaLiberacionSap] [datetime2] NULL,
	[TipoSolp_Id] [int] NULL,

    [PasoCompletado] INT NULL, 
    [EstadoPasos] VARCHAR(20) NULL, 
    CONSTRAINT [PK_dbo.Solp] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	CONSTRAINT [FK_Solp_Pliego] FOREIGN KEY (Pliego_id) REFERENCES [Pliego]([Id]),
	CONSTRAINT [FK_Solp_TablaSap_ClaseDocumento] FOREIGN KEY (ClaseDocumento_Id) REFERENCES [TablaSap]([Id]),
	CONSTRAINT [FK_Solp_TablaSap_EstadoSolpSap] FOREIGN KEY (EstadoSolpSap_Id) REFERENCES [TablaSap]([Id]),
	CONSTRAINT [FK_Solp_TablaEstado_EstadoDocumento] FOREIGN KEY (EstadoDocumento_Id) REFERENCES [TablaEstado]([Id]),
	CONSTRAINT [FK_Solp_TablaGeneral_TipoSolp] FOREIGN KEY (TipoSolp_Id) REFERENCES [TablaGeneral]([Id]),
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
