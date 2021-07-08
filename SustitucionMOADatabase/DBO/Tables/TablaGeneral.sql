CREATE TABLE [dbo].[TablaGeneral]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Tabla] [nvarchar](max) NOT NULL,
	[Codigo] [nvarchar](max) NOT NULL,
	[Descripcion] [nvarchar](max) NOT NULL,
	[Padre_id] [int] NULL,
CONSTRAINT [PK_dbo.TablaGeneral] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	CONSTRAINT [FK_TablaGeneral_TablaGeneral] FOREIGN KEY (Padre_id) REFERENCES [TablaGeneral]([Id]),
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
