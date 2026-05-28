CREATE TABLE [dbo].[FiscalAprobador]
(
	[UsuarioId] INT NOT NULL PRIMARY KEY, 
    CONSTRAINT [FK_FiscalAprobador_Usuario] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuario]([Id])
)
