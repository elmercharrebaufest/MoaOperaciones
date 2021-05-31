CREATE TABLE [dbo].[PliegoArchivo]
(
	[Archivo_Id] [int] NOT NULL,
	[Pliego_Id] [int] NOT NULL,

CONSTRAINT [PK_dbo.PliegoArchivo] PRIMARY KEY CLUSTERED 
(
	[Archivo_Id] ASC,
	[Pliego_Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	CONSTRAINT [FK_PliegoArchivo_Archivo] FOREIGN KEY (Archivo_Id) REFERENCES [Archivo]([Id]),
	CONSTRAINT [FK_PliegoArchivo_Pliego] FOREIGN KEY (Pliego_Id) REFERENCES [Pliego]([Id]),
) ON [PRIMARY]
