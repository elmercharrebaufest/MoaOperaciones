CREATE TABLE [dbo].[LogPesificacion]
(
	[Id] INT NOT NULL IDENTITY, 
    [IdUsuario] INT NOT NULL, 
    [Fecha] SMALLDATETIME NOT NULL, 
    [Contrato] INT NULL, 
    [Fijacion] INT NULL, 
    [CantidadKilos] DECIMAL(18, 4) NULL, 
    [CodigoProveedor] NVARCHAR(50) NOT NULL, 
    [EsCargaMasiva] BIT NOT NULL, 
    [RutaFisicaArchivo] NVARCHAR(500) NULL, 
    [IdArchivo] INT NULL, 
    [EnvioExitoso] BIT NOT NULL, 
    CONSTRAINT [PK_Pesificacion] PRIMARY KEY ([Id]), 
    CONSTRAINT [FK_Pesificacion_Usaurio] FOREIGN KEY ([IdUsuario]) REFERENCES [Usuario]([Id]),
    CONSTRAINT [FK_Pesificacion_Archivo] FOREIGN KEY ([IdArchivo]) REFERENCES [Archivo]([Id]) 
)
