CREATE TABLE [dbo].[DistanciaDomicilio]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [DomicilioDescripcion] VARCHAR(250) NOT NULL, 
    [DireccionBuscada] VARCHAR(200) NOT NULL, 
    [DistanciaKm] INT NULL, 
    [JsonLugaresOSM] VARCHAR(MAX) NULL, 
    [JsonRutaOSRM] VARCHAR(MAX) NULL
)
