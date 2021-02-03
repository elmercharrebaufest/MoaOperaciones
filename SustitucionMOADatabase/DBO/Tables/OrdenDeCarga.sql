CREATE TABLE [dbo].[OrdenDeCarga]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Cliente_Id] INT NOT NULL, 
    [FechaCarga] DATETIME NOT NULL, 
    [CUITCliente] INT NOT NULL, 
    [NombreChofer] VARCHAR(30) NOT NULL, 
    [ApellidoChofer] VARCHAR(30) NOT NULL, 
    [CUITChofer] INT NOT NULL, 
    [CUITTransporte] INT NOT NULL, 
    [RazonSocialTransporte] VARCHAR(100) NOT NULL, 
    [Producto] VARCHAR(50) NOT NULL, 
    [Cantidad] INT NOT NULL, 
    [Estado] INT NOT NULL, 
    [ContratoSAP] VARCHAR(15) NOT NULL, 
    [CorredorSeleccionado] BIT NOT NULL, 
    [TransporteExiste] BIT NOT NULL, 
    CONSTRAINT [FK_OrdenDeCarga_Proveedor] FOREIGN KEY (Cliente_Id) REFERENCES Proveedor(Id), 
)
