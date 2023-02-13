CREATE TABLE [dbo].[Cosecha]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [Nombre] NVARCHAR(20) NOT NULL,
    [Inicio] DATE NOT NULL,
    [Fin] DATE NOT NULL, 
    [EnviarATSA] BIT NOT NULL DEFAULT 1, 
    [PermitirAltas] BIT NOT NULL DEFAULT 1,
)
