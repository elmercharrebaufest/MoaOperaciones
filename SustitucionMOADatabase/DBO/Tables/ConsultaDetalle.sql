CREATE TABLE [dbo].[ConsultaDetalle]
(
	[Id] INT IDENTITY(1,1) NOT NULL,
	[Consulta_Id] INT NOT NULL,
	[Fecha] DATETIME2 NULL, --FechaPago, FechaFactura, FechaOblea
	[ComprobanteNo] NVARCHAR(max) NULL, --SalidaPagoNo, FacturaNo
	[ContratoNo] NVARCHAR(max) NULL,
	[Impuesto] DECIMAL(18, 3) NULL, --ImpuestoRetenido, ImpuestoPercibido, Impuesto
	[Importe] DECIMAL(18, 3) NULL, --ImporteRetencion
	[CausaConsulta_Id] INT NOT NULL,
	[BolsaEmisoraOblea] NVARCHAR(max) NULL,
	
CONSTRAINT [PK_dbo.ConsultaDetalle] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO