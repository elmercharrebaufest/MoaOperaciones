CREATE TABLE [dbo].[ArchivoCampoSustentable]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [CampoCosechaId] INT NOT NULL, 
    [ProveedorId] INT NOT NULL, 
    [ProcesadoUcropit] BIT NOT NULL DEFAULT 0, 
    [IdArchivoRecepcion] INT NULL
)
