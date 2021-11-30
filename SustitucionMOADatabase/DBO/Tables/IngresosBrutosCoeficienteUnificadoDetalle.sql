CREATE TABLE [dbo].[IngresosBrutosCoeficienteUnificadoDetalle]
(
	[Id] INT IDENTITY(1,1) NOT NULL, 
	[IngresosBrutosCoeficienteUnificado_Id] INT NOT NULL, 
    [NumeroJurisdiccion] INT NULL, 
    [Jurisdiccion] VARCHAR(MAX) NULL, 
    [FechaInicio] DATETIME NULL, 
    [FechaCese] DATETIME NULL, 
    [CoeficienteIngresos] DECIMAL(10, 4) NULL, 
    [CoeficienteGastos] DECIMAL(10, 4) NULL, 
    [CoeficienteUnificado] DECIMAL(10, 4) NULL,
    [FechaUltimaModificacion] DATETIME NOT NULL, 
	CONSTRAINT [PK_dbo.IngresosBrutosCoeficienteUnificadoDetalle] PRIMARY KEY CLUSTERED 
    (
		[Id] ASC
	) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[IngresosBrutosCoeficienteUnificadoDetalle]  
    WITH CHECK ADD CONSTRAINT [FK_dbo.IngresosBrutosCoeficienteUnificadoDetalle_dbo.IngresosBrutosCoeficienteUnificado] 
    FOREIGN KEY([IngresosBrutosCoeficienteUnificado_Id])
    REFERENCES [dbo].[IngresosBrutosCoeficienteUnificado] ([Id])
GO

ALTER TABLE [dbo].[IngresosBrutosCoeficienteUnificadoDetalle] CHECK CONSTRAINT [FK_dbo.IngresosBrutosCoeficienteUnificadoDetalle_dbo.IngresosBrutosCoeficienteUnificado]
GO