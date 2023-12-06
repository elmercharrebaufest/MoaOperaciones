CREATE TABLE [dbo].[AplicacionCartaPorteCargaMasivaFila]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [AplicacionCartaPorteCargaMasivaId] INT NOT NULL, 
    [FilaNumero] SMALLINT NOT NULL, 
    [Contrato] VARCHAR(50) NULL, 
    [CartaPorte] VARCHAR(50) NULL, 
    [Kilos] VARCHAR(50) NULL, 
    [Error] VARCHAR(200) NULL, 
    CONSTRAINT [FK_AplicacionCartaPorteCargaMasivaFila_AplicacionCartaPorteCargaMasiva] FOREIGN KEY ([AplicacionCartaPorteCargaMasivaId]) REFERENCES [AplicacionCartaPorteCargaMasiva]([Id])
)
