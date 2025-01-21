CREATE TABLE [dbo].[ResultadoAnalisisOcr]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    Archivo_Id INT NOT NULL,         
    Usuario_Id INT NOT NULL,         
    FechaAlta DATETIME NOT NULL,     
    IsValid BIT NOT NULL,            
    Message NVARCHAR(500),           
    ValidataionType NVARCHAR(100),   
    Value NVARCHAR(500),             
    Input NVARCHAR(MAX),             
    CONSTRAINT FK_ResultadoAnalisisOcr_Archivo FOREIGN KEY (Archivo_Id) REFERENCES Archivo(Id),
    CONSTRAINT FK_ResultadoAnalisisOcr_Usuario FOREIGN KEY (Usuario_Id) REFERENCES Usuario(Id)
)
