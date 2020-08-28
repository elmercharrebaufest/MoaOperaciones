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
    CONSTRAINT [PK_dbo.Proveedor] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

