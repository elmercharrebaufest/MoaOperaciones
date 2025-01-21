CREATE TABLE [dbo].[ResultadoOcr]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    Texto NVARCHAR(MAX),
    Archivo_Id INT NOT NULL,
    Usuario_Id INT NOT NULL,
    FechaAlta DATETIME NOT NULL,    
    CONSTRAINT FK_ResultadoOcr_Archivo FOREIGN KEY (Archivo_Id) REFERENCES Archivo(Id),
    CONSTRAINT FK_ResultadoOcr_Usuario FOREIGN KEY (Usuario_Id) REFERENCES Usuario(Id)
)
