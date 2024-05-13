CREATE TABLE [dbo].[CentroDireccion]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CodigoSap] [nvarchar](max) NULL,
	[Direccion] [nvarchar](max) NULL,
	[Numero] [nvarchar](max) NULL,
	[Cp] [nvarchar](max) NULL,
	[Pais] [nvarchar](max) NULL,
	[RegionSap_Id] INT NULL, 	
	CONSTRAINT [FK.CentroDireccion_RegionSap_RegionSAP_Id] FOREIGN KEY ([RegionSAP_Id]) REFERENCES [RegionSap]([Id]),
    CONSTRAINT [PK_dbo.CentroDireccion] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
