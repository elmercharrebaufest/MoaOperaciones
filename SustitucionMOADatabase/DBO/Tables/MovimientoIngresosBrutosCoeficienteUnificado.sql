CREATE TABLE [dbo].[MovimientoIngresosBrutosCoeficienteUnificado]
(
	[Id] INT NOT NULL IDENTITY(1,1),
	[IngresosBrutosCoeficienteUnificado_Id] INT NOT NULL,
	[Observaciones] VARCHAR(MAX) NOT NULL,
	[Fecha] DATETIME NOT NULL,
	[TipoMovimientoIngresosBrutosCoeficienteUnificado_Id] INT NOT NULL,
	[OrigenMovimientoIngresosBrutosCoeficienteUnificado_Id] INT NOT NULL,
    [EstadoAnterior_Id] INT NOT NULL,
    [EstadoPosterior_Id] INT NOT NULL,
    CONSTRAINT [PK_dbo.MovimientoIngresosBrutosCoeficienteUnificado] PRIMARY KEY CLUSTERED
	(
		[Id] ASC
	) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[MovimientoIngresosBrutosCoeficienteUnificado]  
    WITH CHECK ADD CONSTRAINT [FK_dbo.MovimientoIngresosBrutosCoeficienteUnificado_dbo.IngresosBrutosCoeficienteUnificado]
    FOREIGN KEY([IngresosBrutosCoeficienteUnificado_Id])
    REFERENCES [dbo].[IngresosBrutosCoeficienteUnificado] ([Id])
GO

ALTER TABLE [dbo].[MovimientoIngresosBrutosCoeficienteUnificado]  WITH CHECK 
	ADD CONSTRAINT [FK_dbo.MovimientoIngresosBrutosCoeficienteUnificado_dbo.TipoMovimientoIngresosBrutosCoeficienteUnificado] 
	FOREIGN KEY([TipoMovimientoIngresosBrutosCoeficienteUnificado_Id])
	REFERENCES [dbo].[TipoMovimientoIngresosBrutosCoeficienteUnificado]([Id])
GO

ALTER TABLE [dbo].[MovimientoIngresosBrutosCoeficienteUnificado]  WITH CHECK 
	ADD CONSTRAINT [FK_dbo.MovimientoIngresosBrutosCoeficienteUnificado_dbo.OrigenMovimientoIngresosBrutosCoeficienteUnificado] 
	FOREIGN KEY([OrigenMovimientoIngresosBrutosCoeficienteUnificado_Id])
	REFERENCES [dbo].[OrigenMovimientoIngresosBrutosCoeficienteUnificado]([Id])
GO

ALTER TABLE [dbo].[MovimientoIngresosBrutosCoeficienteUnificado]  WITH CHECK 
	ADD CONSTRAINT [FK_dbo.MovimientoIngresosBrutosCoeficienteUnificado_dbo.EstadoIngresosBrutosCoeficienteUnificado_Anterior] 
	FOREIGN KEY([EstadoAnterior_Id])
	REFERENCES [dbo].[EstadoIngresosBrutosCoeficienteUnificado]([Id])
GO

ALTER TABLE [dbo].[MovimientoIngresosBrutosCoeficienteUnificado]  WITH CHECK 
	ADD CONSTRAINT [FK_dbo.MovimientoIngresosBrutosCoeficienteUnificado_dbo.EstadoIngresosBrutosCoeficienteUnificado_Posterior] 
	FOREIGN KEY([EstadoPosterior_Id])
	REFERENCES [dbo].[EstadoIngresosBrutosCoeficienteUnificado]([Id])
GO