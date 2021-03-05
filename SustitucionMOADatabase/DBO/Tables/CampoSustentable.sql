CREATE TABLE [dbo].[CampoSustentable]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [Nombre] NVARCHAR(50) NOT NULL,
    [Pais] NVARCHAR(50) NOT NULL,
    [Provincia] NVARCHAR(50) NOT NULL,
    [Localidad] NVARCHAR(50) NOT NULL
)
