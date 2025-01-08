CREATE TABLE [dbo].[TablaSap]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Tabla] [nvarchar](400) NOT NULL, -- se limita el tamaño para permitir la creación de índice (https://learn.microsoft.com/en-us/sql/sql-server/maximum-capacity-specifications-for-sql-server)
	[Codigo] [nvarchar](400) NULL,
	[CodigoSap] [nvarchar](400) NULL,
	[Descripcion] [nvarchar](max) NULL,
	[Padre_id] [int] NULL,
	[FiltroComprador] BIT NULL, 
	[Deshabilitado] BIT NULL, 
	CONSTRAINT [PK_dbo.TablaSap] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
	CONSTRAINT [FK_TablaSap_TablaSap] FOREIGN KEY (Padre_id) REFERENCES [TablaSap]([Id]),
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

CREATE NONCLUSTERED INDEX [IX_TablaSap_Tabla] ON [dbo].[TablaSap](Tabla)

GO

CREATE NONCLUSTERED INDEX [IX_TablaSap_Codigo] ON [dbo].[TablaSap](Codigo) INCLUDE ([id])

GO

CREATE NONCLUSTERED INDEX [IX_TablaSap_CodigoSap] ON [dbo].[TablaSap](CodigoSap) INCLUDE ([id])

GO
