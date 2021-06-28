SET NOCOUNT ON
BEGIN TRAN

DECLARE @idCategoria INT

IF NOT EXISTS(SELECT 1 FROM Categoria WHERE Code = 'PAR')
BEGIN
	INSERT INTO Categoria(Code, Nombre)
	VALUES ('PAR', 'Parcial')

	SET @idCategoria = SCOPE_IDENTITY()

	INSERT INTO SubCategoria(Code, Nombre, Categoria_Id)
	VALUES('NROR', 'No registradas, Observadas y Rechazadas', @idCategoria)
END

IF NOT EXISTS(SELECT 1 FROM Categoria WHERE Code = 'PARDIR')
BEGIN
	INSERT INTO Categoria(Code, Nombre)
	VALUES ('PARDIR', 'Parcial Directo')

	SET @idCategoria = SCOPE_IDENTITY()

	INSERT INTO SubCategoria(Code, Nombre, Categoria_Id)
	VALUES('NROR', 'No registradas, Observadas y Rechazadas', @idCategoria)
END

IF NOT EXISTS(SELECT 1 FROM Categoria WHERE Code = 'PARCOR')
BEGIN
	INSERT INTO Categoria(Code, Nombre)
	VALUES ('PARCOR', 'Parcial Corredor')

	SET @idCategoria = SCOPE_IDENTITY()

	INSERT INTO SubCategoria(Code, Nombre, Categoria_Id)
	VALUES('NROR', 'No registradas, Observadas y Rechazadas', @idCategoria)
END


IF NOT EXISTS(SELECT 1 FROM Categoria WHERE Code = 'FIN')
BEGIN
	INSERT INTO Categoria(Code, Nombre)
	VALUES ('FIN', 'Final')

	SET @idCategoria = SCOPE_IDENTITY()

	INSERT INTO SubCategoria(Code, Nombre, Categoria_Id)
	VALUES('NROR', 'No registradas, Observadas y Rechazadas', @idCategoria)

	INSERT INTO SubCategoria(Code, Nombre, Categoria_Id)
	VALUES('PROF', 'Proforma', @idCategoria)

	INSERT INTO SubCategoria(Code, Nombre, Categoria_Id)
	VALUES('SERV', 'Servicios', @idCategoria)

	INSERT INTO SubCategoria(Code, Nombre, Categoria_Id)
	VALUES('BON', 'Bonificaciones', @idCategoria)

	INSERT INTO SubCategoria(Code, Nombre, Categoria_Id)
	VALUES('CDG', 'Certificaciones de Granos', @idCategoria)
END

IF NOT EXISTS(SELECT 1 FROM Categoria WHERE Code = 'FINDIR')
BEGIN
	INSERT INTO Categoria(Code, Nombre)
	VALUES ('FINDIR', 'Final Directo')

	SET @idCategoria = SCOPE_IDENTITY()

	INSERT INTO SubCategoria(Code, Nombre, Categoria_Id)
	VALUES('NROR', 'No registradas, Observadas y Rechazadas', @idCategoria)

	INSERT INTO SubCategoria(Code, Nombre, Categoria_Id)
	VALUES('PROF', 'Proforma', @idCategoria)

	INSERT INTO SubCategoria(Code, Nombre, Categoria_Id)
	VALUES('SERV', 'Servicios', @idCategoria)

	INSERT INTO SubCategoria(Code, Nombre, Categoria_Id)
	VALUES('BON', 'Bonificaciones', @idCategoria)

	INSERT INTO SubCategoria(Code, Nombre, Categoria_Id)
	VALUES('CDG', 'Certificaciones de Granos', @idCategoria)
END

IF NOT EXISTS(SELECT 1 FROM Categoria WHERE Code = 'FINCOR')
BEGIN
	INSERT INTO Categoria(Code, Nombre)
	VALUES ('FINCOR', 'Final Corredor')

	SET @idCategoria = SCOPE_IDENTITY()

	INSERT INTO SubCategoria(Code, Nombre, Categoria_Id)
	VALUES('NROR', 'No registradas, Observadas y Rechazadas', @idCategoria)

	INSERT INTO SubCategoria(Code, Nombre, Categoria_Id)
	VALUES('PROF', 'Proforma', @idCategoria)

	INSERT INTO SubCategoria(Code, Nombre, Categoria_Id)
	VALUES('SERV', 'Servicios', @idCategoria)

	INSERT INTO SubCategoria(Code, Nombre, Categoria_Id)
	VALUES('BON', 'Bonificaciones', @idCategoria)

	INSERT INTO SubCategoria(Code, Nombre, Categoria_Id)
	VALUES('CDG', 'Certificaciones de Granos', @idCategoria)
END

IF NOT EXISTS(SELECT 1 FROM Categoria WHERE Code = 'CAL')
BEGIN
	INSERT INTO Categoria(Code, Nombre)
	VALUES ('CAL', 'Calidades')
END

IF NOT EXISTS(SELECT 1 FROM Categoria WHERE Code = 'COM')
BEGIN
	INSERT INTO Categoria(Code, Nombre)
	VALUES ('COM', 'Comisiones')
END

IF NOT EXISTS(SELECT 1 FROM Categoria WHERE Code = 'COMP')
BEGIN
	INSERT INTO Categoria(Code, Nombre)
	VALUES ('COMP', 'Comprobantes')
END

IF NOT EXISTS(SELECT 1 FROM Categoria WHERE Code = 'APP')
BEGIN
	INSERT INTO Categoria(Code, Nombre)
	VALUES ('APP', 'Aplicaciones')
END


IF NOT EXISTS(SELECT 1 FROM Categoria WHERE Code = 'PES')
BEGIN
	INSERT INTO Categoria(Code, Nombre)
	VALUES ('PES', 'Pesificaciones')
END

IF NOT EXISTS(SELECT 1 FROM Categoria WHERE Code = 'PAG')
BEGIN
	INSERT INTO Categoria(Code, Nombre)
	VALUES ('PAG', 'Pagos')
END

IF NOT EXISTS(SELECT 1 FROM Categoria WHERE Code = 'FWEB')
BEGIN
	INSERT INTO Categoria(Code, Nombre)
	VALUES ('FWEB', 'Funcionamiento Web')
END

IF NOT EXISTS(SELECT 1 FROM Categoria WHERE Code = 'MATBA')
BEGIN
	INSERT INTO Categoria(Code, Nombre)
	VALUES ('MATBA', 'Operaciones MATBA')

	SET @idCategoria = SCOPE_IDENTITY()

	INSERT INTO SubCategoria(Code, Nombre, Categoria_Id)
	VALUES('REC', 'Recibos', @idCategoria)

	INSERT INTO SubCategoria(Code, Nombre, Categoria_Id)
	VALUES('FAL', 'Faltantes/Excedentes', @idCategoria)

	INSERT INTO SubCategoria(Code, Nombre, Categoria_Id)
	VALUES('CAL', 'Calidades', @idCategoria)
END

IF NOT EXISTS(SELECT 1 FROM Categoria WHERE Code = 'PROVG')
BEGIN
	INSERT INTO Categoria(Code, Nombre)
	VALUES ('PROVG', 'Proveedores generales')

	SET @idCategoria = SCOPE_IDENTITY()

	INSERT INTO SubCategoria(Code, Nombre, Categoria_Id)
	VALUES('CNV', 'Comprobantes no visualizados', @idCategoria)

	INSERT INTO SubCategoria(Code, Nombre, Categoria_Id)
	VALUES('CEG', 'Comprobantes en gestión', @idCategoria)

	INSERT INTO SubCategoria(Code, Nombre, Categoria_Id)
	VALUES('VENC', 'Vencimiento de comprobantes', @idCategoria)

	INSERT INTO SubCategoria(Code, Nombre, Categoria_Id)
	VALUES('POTR', 'Otros', @idCategoria)
END

IF NOT EXISTS(SELECT 1 FROM Categoria WHERE Code = 'FLET')
BEGIN
	INSERT INTO Categoria(Code, Nombre)
	VALUES ('FLET', 'Fletes')

	SET @idCategoria = SCOPE_IDENTITY()

	INSERT INTO SubCategoria(Code, Nombre, Categoria_Id)
	VALUES('PDF', 'Proforma de fletes', @idCategoria)

	INSERT INTO SubCategoria(Code, Nombre, Categoria_Id)
	VALUES('CCP', 'Consulta Carta de Porte', @idCategoria)
END

IF NOT EXISTS(SELECT 1 FROM Categoria WHERE Code = 'OTRO')
BEGIN
	INSERT INTO Categoria(Code, Nombre)
	VALUES ('OTRO', 'Otros')
END

commit TRAN