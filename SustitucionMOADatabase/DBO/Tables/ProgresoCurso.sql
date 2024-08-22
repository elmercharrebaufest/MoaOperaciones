CREATE TABLE [dbo].[ProgresoCurso]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
	[UsuarioId] INT NOT NULL,
	[CursoId] INT NOT NULL,
	[DetalleProgreso] VARCHAR(MAX) NULL, 
    [FechaInicio] DATETIME NULL,
	[FechaUltimoIntento] DATETIME NULL,
	[FechaCompletado] DATETIME NULL,
	[MinutosCursados] INT NULL,
	CONSTRAINT [FK_Usuario_ProgresoCurso] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuario]([Id]),
	CONSTRAINT [FK_Curso_ProgresoCurso] FOREIGN KEY ([CursoId]) REFERENCES [Curso]([Id]),
)
