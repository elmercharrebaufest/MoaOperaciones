CREATE TABLE [dbo].[CampoSustentable]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [Nombre] NVARCHAR(50) NOT NULL,
    [Localidad_Id] INT NOT NULL, 
    CONSTRAINT [FK_CampoSustentable_Localidad] FOREIGN KEY (Localidad_Id) REFERENCES [dbo].Localidad(LocalidadId)
)
