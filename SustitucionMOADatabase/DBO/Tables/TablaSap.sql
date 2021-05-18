CREATE TABLE [dbo].[TablaSap]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Tabla] [nvarchar](max) NOT NULL,
	[Codigo] [nvarchar](max) NULL,
	[CodigoSap] [nvarchar](max) NULL,
	[Descripcion] [nvarchar](max) NULL,
CONSTRAINT [PK_dbo.TablaSap] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
