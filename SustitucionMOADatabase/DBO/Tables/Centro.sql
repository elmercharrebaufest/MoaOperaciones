CREATE TABLE [dbo].[Centro]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CodigoSap] [nvarchar](max) NULL,
	[Descripcion] [nvarchar](max) NULL,
	[Nombre] [nvarchar](max) NULL,
	[Calle] [nvarchar](max) NULL,
	[Cp] [nvarchar](max) NULL,
	[Pais] [nvarchar](max) NULL,
CONSTRAINT [PK_dbo.Centro] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
