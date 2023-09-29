CREATE TABLE [dbo].[ProveedorAuditoria](
	[Id] INT IDENTITY(1,1) NOT NULL,
    [Usuario_Id] INT NULL,
    [Proveedor_Id] INT NULL, 
    [CodigoProveedor] VARCHAR(200) NULL, 
    [Mail] VARCHAR(200) NULL, 
    [Cuit] VARCHAR(200) NULL, 
    [TipoProveedor_Id] INT NULL, 
    [RazonSocial] VARCHAR(200) NULL, 
    [FechaActualizacion] DATETIME NULL, 
    [UsuarioActualizacion] VARCHAR(200) NULL,
    [EsRevendedor] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_dbo.ProveedorAuditoria] PRIMARY KEY CLUSTERED 
   (
	[Id] ASC
   )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) 
GO
