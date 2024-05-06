CREATE TABLE [dbo].[EstadoOrdenResiduos]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Nombre] VARCHAR(40) NOT NULL, 
    [NombreExterno] VARCHAR(40) NOT NULL, 
    [Semaforo] VARCHAR(15) NULL
)
