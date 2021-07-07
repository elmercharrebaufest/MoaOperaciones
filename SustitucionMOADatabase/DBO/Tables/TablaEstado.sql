CREATE TABLE [dbo].[TablaEstado]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Tabla] [nvarchar](max) NOT NULL,
	[Codigo] [nvarchar](max) NOT NULL,
	[Descripcion] [nvarchar](max) NOT NULL,
	[Orden] [int] NOT NULL,
	[Color] [nvarchar](max) NULL,
CONSTRAINT [PK_dbo.TablaEstado] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
