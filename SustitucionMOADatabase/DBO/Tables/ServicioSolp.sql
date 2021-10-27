CREATE TABLE [dbo].[ServicioSolp]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[Codigo] VARCHAR(MAX) NOT NULL,
	[CodigoSap] INT NOT NULL,
	[GrupoArticulos] INT NULL,
	[TipoServicio] VARCHAR(MAX) NOT NULL,
	[AmbitoServicio] VARCHAR(MAX) NOT NULL,
	[Edicion] INT NOT NULL,
	[UnidadMedidaBase] VARCHAR(MAX) NOT NULL,
	[SSCItem] VARCHAR(MAX) NOT NULL, 
    [Descripcion] VARCHAR(MAX) NOT NULL
)