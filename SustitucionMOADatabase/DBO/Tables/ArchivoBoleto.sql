CREATE TABLE [dbo].[ArchivoBoleto]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1), 
    [CUIT] NCHAR(25) NOT NULL, 
    [NombreArchivo] NCHAR(255) NOT NULL, 
    [FechaCarga] DATETIME  DEFAULT (getdate()) NOT NULL, 
    [EstadoArchivoBoleto_Id] INT DEFAULT 1 NOT NULL, 
    [FechaActualizacion] DATETIME NULL, 
    [UsuarioCreacion_Id] INT NOT NULL, 
    [DescripcionActualizacion] NVARCHAR(MAX) NULL, 
    CONSTRAINT [FK_EstadoArchivoBoleto_ArchivoBoleto] FOREIGN KEY ([EstadoArchivoBoleto_Id]) REFERENCES [EstadoArchivoBoleto]([Id]), 
    CONSTRAINT [FK_Usuario_ArchivoBoleto] FOREIGN KEY ([UsuarioCreacion_Id]) REFERENCES [Usuario]([Id])
)
