CREATE TABLE [dbo].[CotizacionArchivo](
   
	[Cotizacion_Id] [int] NOT NULL,
	[Archivo_Id] [int] NOT NULL, 
   
   
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[CotizacionArchivo]  WITH CHECK ADD  CONSTRAINT [FK_dbo.CotizacionArchivo_dbo.Cotizacion_Cotizacion_Id] FOREIGN KEY([Cotizacion_Id])
REFERENCES [dbo].[Cotizacion] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[CotizacionArchivo] CHECK CONSTRAINT [FK_dbo.CotizacionArchivo_dbo.Cotizacion_Cotizacion_Id]
GO

ALTER TABLE [dbo].[CotizacionArchivo]  WITH CHECK ADD  CONSTRAINT [FK_dbo.CotizacionArchivo_dbo.Archivo_Archivo_Id] FOREIGN KEY([Archivo_Id])
REFERENCES [dbo].[Archivo] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[CotizacionArchivo] CHECK CONSTRAINT [FK_dbo.CotizacionArchivo_dbo.Archivo_Archivo_Id]
GO

