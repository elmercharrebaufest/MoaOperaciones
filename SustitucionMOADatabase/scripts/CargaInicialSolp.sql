SET NOCOUNT ON
BEGIN TRAN

-- Tabla estado
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.TablaEstado WHERE Tabla = 'EstadoDocumento' and Codigo = 'INCOMPLETO')
BEGIN
	INSERT INTO TablaEstado(Tabla, Codigo, Descripcion, Orden, Color)
	VALUES('EstadoDocumento', 'INCOMPLETO', 'Incompleto', 0, '#F0AD4E')
END
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.TablaEstado WHERE Tabla = 'EstadoDocumento' and Codigo = 'CREADO')
BEGIN
	INSERT INTO TablaEstado(Tabla, Codigo, Descripcion, Orden, Color)
	VALUES('EstadoDocumento', 'CREADO', 'Creado', 10, '#5CB85C')
END
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.TablaEstado WHERE Tabla = 'EstadoDocumento' and Codigo = 'FINALIZADO')
BEGIN
	INSERT INTO TablaEstado(Tabla, Codigo, Descripcion, Orden, Color)
	VALUES('EstadoDocumento', 'FINALIZADO', 'Finalizado', 20, '#5CB85C')
END

-- Tabla estadoSap

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.TablaSap WHERE Tabla = 'EstadoSolpSap' and Codigo = 'CREADA')
BEGIN
	INSERT INTO TablaSap(Tabla, Codigo, CodigoSap, Descripcion, Padre_id)
	VALUES('EstadoSolpSap', 'CREADA', 'CREADA', 'Creada', null)
END

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.TablaSap WHERE Tabla = 'EstadoSolpSap' and Codigo = 'LIBERADA')
BEGIN
	INSERT INTO TablaSap(Tabla, Codigo, CodigoSap, Descripcion, Padre_id)
	VALUES('EstadoSolpSap', 'LIBERADA', 'LIBERADA', 'Liberada', null)
END

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.TablaSap WHERE Tabla = 'EstadoSolpSap' and Codigo = 'PARC_LIBERADA')
BEGIN
	INSERT INTO TablaSap(Tabla, Codigo, CodigoSap, Descripcion, Padre_id)
	VALUES('EstadoSolpSap', 'PARC_LIBERADA', 'PARC_LIBERADA', ' Parcialmente Liberada', null)
END

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.TablaSap WHERE Tabla = 'EstadoSolpSap' and Codigo = 'RELAC_PEDIDO_COMPRA')
BEGIN
	INSERT INTO TablaSap(Tabla, Codigo, CodigoSap, Descripcion, Padre_id)
	VALUES('EstadoSolpSap', 'RELAC_PEDIDO_COMPRA', 'RELAC_PEDIDO_COMPRA', 'Relac. a pedido compra', null)
END

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.TablaSap WHERE Tabla = 'EstadoSolpSap' and Codigo = 'FINALIZADA')
BEGIN
	INSERT INTO TablaSap(Tabla, Codigo, CodigoSap, Descripcion, Padre_id)
	VALUES('EstadoSolpSap', 'FINALIZADA', 'FINALIZADA', 'Finalizada', null)
END


COMMIT TRAN