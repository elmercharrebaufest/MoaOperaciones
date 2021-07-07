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
	VALUES('EstadoDocumento', 'CREADO', 'Creado', 10, '##5CB85C')
END
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.TablaEstado WHERE Tabla = 'EstadoDocumento' and Codigo = 'FINALIZADO')
BEGIN
	INSERT INTO TablaEstado(Tabla, Codigo, Descripcion, Orden, Color)
	VALUES('EstadoDocumento', 'FINALIZADO', 'Finalizado', 20, '##5CB85C')
END

COMMIT TRAN