CREATE TABLE [dbo].[ProgresoCurso]
(
	[Id] INT NOT NULL PRIMARY KEY,
	[UsuarioId] INT NOT NULL,
	[CursoId] INT NOT NULL,
	[DetalleProgreso] NCHAR(255) NULL, 
    [FechaInicio] DATETIME NULL,
	[FechaUltimoIntento] DATETIME NULL,
	[FechaCompletado] DATETIME NULL,
	CONSTRAINT [FK_Usuario_ProgresoCurso] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuario]([Id]),
	CONSTRAINT [FK_Curso_ProgresoCurso] FOREIGN KEY ([CursoId]) REFERENCES [Curso]([Id]),
)
