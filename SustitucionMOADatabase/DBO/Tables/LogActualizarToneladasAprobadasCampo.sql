CREATE TABLE [dbo].[LogActualizarToneladasAprobadasCampo]
(
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Fecha] datetime,
	[IdCampo] int,
	[IdTSA] int,
	[Cuit] NVARCHAR(15),
	[ToneladasAprobadas] FLOAT,
	[MotivoRechazo] NVARCHAR(500),
	[Log] NVARCHAR(MAX),
	[CosechaNombre] NVARCHAR(40) NULL, 
    CONSTRAINT [PK_dbo.LogActualizarToneladasAprobadasCampo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
