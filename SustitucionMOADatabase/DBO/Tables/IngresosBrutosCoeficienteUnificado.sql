CREATE TABLE [dbo].[IngresosBrutosCoeficienteUnificado]
(
	[Id] INT IDENTITY(1,1) NOT NULL, 
    [EstadoIngresosBrutosCoeficienteUnificado_Id] INT NOT NULL,
    [Anticipo] INT NOT NULL, 
    [Cuit] NVARCHAR(30) NOT NULL, 
    [Sede] INT NOT NULL,
	[FechaCarga] DATETIME NOT NULL, 
    [FechaUltimaModificacion] DATETIME NOT NULL, 
    [Consulta_Id] INT NULL, 
    [Archivo_Id] INT NOT NULL, 
	[SecuenciaIngresosBrutosCoeficienteUnificado_Id] INT NULL,
    [MalCargada] BIT NOT NULL, 
    [RazonSocial] NVARCHAR(MAX) NULL, 
    CONSTRAINT [PK_dbo.IngresosBrutosCoeficienteUnificado] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[IngresosBrutosCoeficienteUnificado]  WITH CHECK 
	ADD CONSTRAINT [FK_dbo.IngresosBrutosCoeficienteUnificado_dbo.EstadoIngresosBrutosCoeficienteUnificado] 
	FOREIGN KEY([EstadoIngresosBrutosCoeficienteUnificado_Id])
	REFERENCES [dbo].[EstadoIngresosBrutosCoeficienteUnificado]([Id])
GO

ALTER TABLE [dbo].[IngresosBrutosCoeficienteUnificado]  WITH CHECK 
	ADD CONSTRAINT [FK_dbo.IngresosBrutosCoeficienteUnificado_dbo.Consulta] 
	FOREIGN KEY([Consulta_Id])
	REFERENCES [dbo].[Consulta]([Id])
GO

ALTER TABLE [dbo].[IngresosBrutosCoeficienteUnificado]  WITH CHECK 
	ADD CONSTRAINT [FK_dbo.IngresosBrutosCoeficienteUnificado_dbo.Archivo] 
	FOREIGN KEY([Archivo_Id])
	REFERENCES [dbo].[Archivo]([Id])
GO

ALTER TABLE [dbo].[IngresosBrutosCoeficienteUnificado]  WITH CHECK 
	ADD CONSTRAINT [FK_dbo.IngresosBrutosCoeficienteUnificado_dbo.SecuenciaIngresosBrutosCoeficienteUnificado] 
	FOREIGN KEY([SecuenciaIngresosBrutosCoeficienteUnificado_Id])
	REFERENCES [dbo].[SecuenciaIngresosBrutosCoeficienteUnificado]([Id])
GO

ALTER TABLE [dbo].[IngresosBrutosCoeficienteUnificado] CHECK CONSTRAINT [FK_dbo.IngresosBrutosCoeficienteUnificado_dbo.EstadoIngresosBrutosCoeficienteUnificado]
GO