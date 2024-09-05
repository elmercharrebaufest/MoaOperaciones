CREATE TABLE [dbo].[CampoSustentable]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [IdScato] INT NULL,
    [Nombre] NVARCHAR(500) NOT NULL,
    [Localidad_Id] INT NOT NULL, 
    [Renspa] VARCHAR(15) NULL, 
    CONSTRAINT [FK_CampoSustentable_Localidad] FOREIGN KEY (Localidad_Id) REFERENCES [dbo].Localidad(LocalidadId)
)
