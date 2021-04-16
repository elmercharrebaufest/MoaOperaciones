CREATE TABLE [dbo].[LiquidacionInformada]
(
    [Id] UNIQUEIDENTIFIER NOT NULL,
    [Proveedor_Id] INT NOT NULL,
    [COE] NVARCHAR(12) NULL,
    [FechaComprobante] DATE NULL,
[FechaInformada] DATE NOT NULL, 
    CONSTRAINT [PK_dbo.LiquidacionInformada] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[LiquidacionInformada]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Liquidacion_dbo.Proveedor_Proveedor_Id] FOREIGN KEY([Proveedor_Id])
REFERENCES [dbo].[Proveedor] ([Id])
GO

ALTER TABLE [dbo].[LiquidacionInformada] CHECK CONSTRAINT [FK_dbo.Liquidacion_dbo.Proveedor_Proveedor_Id]
GO
