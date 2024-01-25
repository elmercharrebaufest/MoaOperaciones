CREATE TABLE [dbo].[RegionSap]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CodigoPais] varchar(3) NOT NULL,
	[CodigoSap] varchar(3) NOT NULL,
	[Descripcion] [nvarchar](200) NOT NULL,
	CONSTRAINT [PK_dbo.RegionSap] PRIMARY KEY CLUSTERED ( [Id] ASC ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
    CONSTRAINT [FK.RegionSap_PaisesSap_CodigoPais] FOREIGN KEY ([CodigoPais]) REFERENCES [PaisesSap]([Codigo])

) 
