CREATE TABLE [dbo].[EstadoIngresosBrutosCoeficienteUnificado]
(
	[Id] INT IDENTITY(1,1) NOT NULL, 
    [Descripcion] VARCHAR(50) NOT NULL,
	CONSTRAINT [PK_dbo.EstadoIngresosBrutosCoeficienteUnificado] PRIMARY KEY CLUSTERED (Id)
)
GO