CREATE TABLE [dbo].[TablaGeneral]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Tabla] [nvarchar](400) NOT NULL,
	[Codigo] [nvarchar](400) NOT NULL,
	[Descripcion] [nvarchar](max) NOT NULL,
	[Padre_id] [int] NULL,
CONSTRAINT [PK_dbo.TablaGeneral] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	CONSTRAINT [FK_TablaGeneral_TablaGeneral] FOREIGN KEY (Padre_id) REFERENCES [TablaGeneral]([Id]),
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

CREATE NONCLUSTERED INDEX [IX_TablaGeneral_Tabla] ON [dbo].[TablaGeneral](Tabla)

GO

CREATE NONCLUSTERED INDEX [IX_TablaGeneral_Codigo] ON [dbo].[TablaGeneral](Codigo) INCLUDE ([id])

GO
