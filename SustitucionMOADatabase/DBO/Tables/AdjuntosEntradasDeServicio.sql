CREATE TABLE [dbo].[AdjuntosEntradasDeServicio]
(
	[Id] INT IDENTITY(1,1) PRIMARY KEY, 
    [NroESTemporal] NVARCHAR(MAX) NULL, 
    [NombreEnBlob] NVARCHAR(MAX) NULL, 
    [NombreArchivo] NVARCHAR(MAX) NULL, 
    [Extension] NVARCHAR(50) NULL
)
