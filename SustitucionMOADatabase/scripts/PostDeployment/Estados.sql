--CotizacionEstado

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.CotizacionEstado WHERE Descripcion = 'Cotizado' ) BEGIN INSERT INTO CotizacionEstado(Descripcion) VALUES('Cotizado') END
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.CotizacionEstado WHERE Descripcion = 'Incompleta' ) BEGIN INSERT INTO CotizacionEstado(Descripcion) VALUES('Incompleta') END
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.CotizacionEstado WHERE Descripcion = 'CotizadoAutomaticamente' ) BEGIN INSERT INTO CotizacionEstado(Descripcion) VALUES('CotizadoAutomaticamente') END

--Estados OC

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.TablaSap WHERE CodigoSap = '0' and Tabla = 'EstadoOC' ) BEGIN INSERT INTO TablaSap(Codigo,CodigoSap,Descripcion,Tabla) VALUES('0','0','Bloqueado','EstadoOC') END
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.TablaSap WHERE CodigoSap = '4' and Tabla = 'EstadoOC' ) BEGIN INSERT INTO TablaSap(Codigo,CodigoSap,Descripcion,Tabla) VALUES('4','4','Liberado PA','EstadoOC') END
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.TablaSap WHERE CodigoSap = '5' and Tabla = 'EstadoOC' ) BEGIN INSERT INTO TablaSap(Codigo,CodigoSap,Descripcion,Tabla) VALUES('5','5','Liberado','EstadoOC') END
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.TablaSap WHERE CodigoSap = '6' and Tabla = 'EstadoOC' ) BEGIN INSERT INTO TablaSap(Codigo,CodigoSap,Descripcion,Tabla) VALUES('6','6','Liberado Primer Nivel','EstadoOC') END
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.TablaSap WHERE CodigoSap = '7' and Tabla = 'EstadoOC' ) BEGIN INSERT INTO TablaSap(Codigo,CodigoSap,Descripcion,Tabla) VALUES('7','7','Liberado Segundo Nivel','EstadoOC') END
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.TablaSap WHERE CodigoSap = '8' and Tabla = 'EstadoOC' ) BEGIN INSERT INTO TablaSap(Codigo,CodigoSap,Descripcion,Tabla) VALUES('8','8','Liberado Tercer Nivel','EstadoOC') END
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.TablaSap WHERE CodigoSap = '9' and Tabla = 'EstadoOC' ) BEGIN INSERT INTO TablaSap(Codigo,CodigoSap,Descripcion,Tabla) VALUES('9','9','Liberado Cuarto Nivel','EstadoOC') END

--EstadoArchivoBoleto

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.EstadoArchivoBoleto WHERE Nombre = 'Pendiente' ) BEGIN INSERT INTO EstadoArchivoBoleto(Id, Nombre, Color) VALUES(1, 'Pendiente', 'yellow') END
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.EstadoArchivoBoleto WHERE Nombre = 'Rechazado' ) BEGIN INSERT INTO EstadoArchivoBoleto(Id, Nombre, Color) VALUES(2, 'Rechazado', 'red') END
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.EstadoArchivoBoleto WHERE Nombre = 'Aceptado' ) BEGIN INSERT INTO EstadoArchivoBoleto(Id, Nombre, Color) VALUES(3, 'Aceptado', 'green') END
