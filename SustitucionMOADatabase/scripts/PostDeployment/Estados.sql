--CotizacionEstado

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.CotizacionEstado WHERE Descripcion = 'Cotizado' ) BEGIN INSERT INTO CotizacionEstado(Descripcion) VALUES('Cotizado') END
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.CotizacionEstado WHERE Descripcion = 'Incompleta' ) BEGIN INSERT INTO CotizacionEstado(Descripcion) VALUES('Incompleta') END
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.CotizacionEstado WHERE Descripcion = 'CotizadoAutomaticamente' ) BEGIN INSERT INTO CotizacionEstado(Descripcion) VALUES('CotizadoAutomaticamente') END