CREATE TABLE [dbo].[CertificacionRegistrada]
(
	[Id] INT NOT NULL IDENTITY (1, 1), 
    [NombreDeArchivo] NVARCHAR(MAX) NOT NULL, 
    [NRO_OC] NCHAR(20) NOT NULL, 
    [NRO_Certificacion] NCHAR(10) NOT NULL, 
    [Importe] DECIMAL(18, 2) NOT NULL, 
    [Moneda] NCHAR(10) NOT NULL, 
    [FechaDeRegistro] DATETIME2 NOT NULL, 
    [UsuarioId] INT NOT NULL,
    [ProveedorId] INT NOT NULL,
    [ArchivoId] INT NOT NULL,
    CONSTRAINT [PK_CertificacionRegistrada] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK.CertificacionRegistrada_Usuario] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuario]([Id]),
    CONSTRAINT [FK.CertificacionRegistrada_Proveedor] FOREIGN KEY ([ProveedorId]) REFERENCES [Proveedor]([Id]),
    CONSTRAINT [FK.CertificacionRegistrada_Archivo] FOREIGN KEY ([ArchivoId]) REFERENCES [Archivo]([Id]),
)