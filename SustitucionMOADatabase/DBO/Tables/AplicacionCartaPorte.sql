CREATE TABLE [dbo].[AplicacionCartaPorte]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [Proveedor_Id] INT NOT NULL, 
    [Contrato] VARCHAR(50) NOT NULL, 
    [CartaPorte] VARCHAR(50) NOT NULL, 
    [Kilogramos] INT NOT NULL, 
    [Estado] INT NOT NULL, 
    [Usuario_Id] INT NOT NULL, 
    [Error] VARCHAR(MAX) NULL, 
    [FechaAlta] DATETIME NOT NULL, 
    [FechaActualizacion] DATETIME NULL, 
    CONSTRAINT [FK_AplicacionCartaPorte_Proveedor] FOREIGN KEY (Proveedor_Id) REFERENCES Proveedor(Id),
    CONSTRAINT [FK_AplicacionCartaPorte_Usuario] FOREIGN KEY (Usuario_Id) REFERENCES Usuario(Id),
)
