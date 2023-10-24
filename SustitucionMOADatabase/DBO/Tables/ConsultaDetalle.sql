CREATE TABLE [dbo].[ConsultaDetalle]
(
	[Id] INT NOT NULL,
	--[Consulta_Id] INT NOT NULL,
	[Fecha] DATETIME2 NULL, --FechaPago, FechaFactura, FechaOblea
	[ComprobanteNo] NVARCHAR(max) NULL, --SalidaPagoNo, FacturaNo
	[ContratoNo] NVARCHAR(max) NULL,
	[Impuesto] NVARCHAR(MAX) NULL, --ImpuestoRetenido, ImpuestoPercibido, Impuesto
	[Importe] DECIMAL(18, 3) NULL, --ImporteRetencion
	[CausaConsulta_Id] INT NULL,
	[BolsaEmisoraOblea] NVARCHAR(max) NULL,
	
[OtroComprobanteNo] NVARCHAR(MAX) NULL, 
    [Material_Id] INT NULL, 
    [Orden_Id] INT NULL, 
    CONSTRAINT [PK_dbo.ConsultaDetalle] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[ConsultaDetalle]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ConsultaDetalle_dbo.Consulta_Consulta_Id] FOREIGN KEY([Id])
REFERENCES [dbo].[Consulta] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ConsultaDetalle] CHECK CONSTRAINT [FK_dbo.ConsultaDetalle_dbo.Consulta_Consulta_Id]
GO

ALTER TABLE [dbo].[ConsultaDetalle]  WITH CHECK ADD  CONSTRAINT [FK_dbo.ConsultaDetalle_dbo.CausaConsulta_CausaConsulta_Id] FOREIGN KEY([CausaConsulta_Id])
REFERENCES [dbo].[CausaConsulta] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[ConsultaDetalle] CHECK CONSTRAINT [FK_dbo.ConsultaDetalle_dbo.CausaConsulta_CausaConsulta_Id]
GO