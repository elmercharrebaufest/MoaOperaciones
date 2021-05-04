CREATE TABLE [dbo].[DeclaracionCampoSustentable]
(
	[Cosecha_Id] INT NOT NULL,
	[Proveedor_Id] INT NOT NULL,
	 [FechaFirma] DATETIME NULL, 
    [OpcionDeclarada] INT NULL, 
    [HectareasDeclaradas] FLOAT NULL, 

  [CUIT] NVARCHAR(15) NOT NULL, 
    [RazonSocial] NVARCHAR(200) NULL, 
    [Archivo_Id] INT NULL, 
    CONSTRAINT [PK_dbo.DeclaracionCampoSustentable] PRIMARY KEY CLUSTERED 
(
	[Cosecha_Id], [CUIT]
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY], 
    CONSTRAINT [FK_DeclaracionCampoSustentable_Cosecha] FOREIGN KEY (Cosecha_Id) REFERENCES [Cosecha]([Id]),
    CONSTRAINT [FK_DeclaracionCampoSustentable_Proveedor] FOREIGN KEY (Proveedor_Id) REFERENCES [Proveedor]([Id])
) ON [PRIMARY]