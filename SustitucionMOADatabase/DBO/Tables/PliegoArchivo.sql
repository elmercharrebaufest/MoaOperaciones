CREATE TABLE [dbo].[PliegoArchivo]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Archivo_Id] [int] NULL,
	[Pliego_Id] [int] NULL,
	[TipoPliegoArchivo_Id] [int] NULL,

CONSTRAINT [PK_dbo.PliegoArchivo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	CONSTRAINT [FK_PliegoArchivo_Archivo] FOREIGN KEY (Archivo_Id) REFERENCES [Archivo]([Id]),
	CONSTRAINT [FK_PliegoArchivo_Pliego] FOREIGN KEY (Pliego_Id) REFERENCES [Pliego]([Id]),
	CONSTRAINT [FK_PliegoArchivo_TablaGeneral_TipoPliegoArchivo] FOREIGN KEY (TipoPliegoArchivo_Id) REFERENCES [TablaGeneral]([Id]),
) ON [PRIMARY]
