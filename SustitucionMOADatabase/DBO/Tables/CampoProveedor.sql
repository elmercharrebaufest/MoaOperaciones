CREATE TABLE [dbo].[CampoProveedor]
(
    [CampoCosecha_Id] INT NOT NULL,
    [Proveedor_Id] INT NOT NULL,
    [HectareasTotales] FLOAT NOT NULL,
    [HectareasSoja] FLOAT NOT NULL,
    [Longitud] FLOAT NOT NULL,
    [Latitud] FLOAT NOT NULL,
    [Archivo_Id] INT NOT NULL,
    [FechaCreacion] DATETIME NULL, 
    [FechaModificacion] DATETIME NULL, 
    PRIMARY KEY(CampoCosecha_Id, Proveedor_Id),
    CONSTRAINT [FK_CampoProveedor_ToArchivo] FOREIGN KEY ([Archivo_Id]) REFERENCES [Archivo]([Id]),
    CONSTRAINT [FK_CampoProveedor_ToCampoCosecha] FOREIGN KEY ([CampoCosecha_Id]) REFERENCES [CampoCosecha]([Id]),
    CONSTRAINT [FK_CampoProveedor_ToProveedor] FOREIGN KEY ([Proveedor_Id]) REFERENCES [Proveedor]([Id]),
)
