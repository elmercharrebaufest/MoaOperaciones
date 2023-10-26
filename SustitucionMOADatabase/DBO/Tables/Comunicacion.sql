CREATE TABLE [dbo].[Comunicacion]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),  
    [ComunicacionTipo] INT NOT NULL, 
    [ProveedorId] NVARCHAR(MAX) NOT NULL, 
    [FechaCreacion] DATETIME NULL, 
    [Leida] BIT NULL, 
    [Detalle] NVARCHAR(MAX) NULL,
    [CM05] varchar(4), 
    [FechaRecomunicacion] DATETIME NULL, 
    [Comprobante] NVARCHAR(20) NULL, 
    [FechaVencimiento] DATETIME NULL, 
    [DescripcionWeb] NVARCHAR(50) NULL 
)
