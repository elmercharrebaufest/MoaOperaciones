SET NOCOUNT ON
BEGIN TRAN

DECLARE @idCategoria INT

IF NOT EXISTS(SELECT 1 FROM Categoria WHERE Code = 'REI')
BEGIN
	INSERT INTO Categoria(Code, Nombre)
	VALUES ('REI', 'Reclamo Impositivo')

	SET @idCategoria = SCOPE_IDENTITY()

	INSERT INTO SubCategoria(Code, Nombre, Categoria_Id)
	VALUES('RET', 'Retenciones', @idCategoria)

	INSERT INTO SubCategoria(Code, Nombre, Categoria_Id)
	VALUES('PER', 'Percepciones', @idCategoria)
END

IF NOT EXISTS(SELECT 1 FROM Categoria WHERE Code = 'BOL')
BEGIN
	INSERT INTO Categoria(Code, Nombre)
	VALUES ('BOL', 'Boletos')

	SET @idCategoria = SCOPE_IDENTITY()

	INSERT INTO SubCategoria(Code, Nombre, Categoria_Id)
	VALUES('CON', 'Contratos', @idCategoria)

	INSERT INTO SubCategoria(Code, Nombre, Categoria_Id)
	VALUES('REG', 'Registraciones', @idCategoria)

	INSERT INTO SubCategoria(Code, Nombre, Categoria_Id)
	VALUES('OPC', 'Oblea Plan Canje', @idCategoria)
END

IF NOT EXISTS(SELECT 1 FROM Categoria WHERE Code = 'ACT')
BEGIN
	INSERT INTO Categoria(Code, Nombre)
	VALUES ('ACT', 'Actualización')

	SET @idCategoria = SCOPE_IDENTITY()

	INSERT INTO SubCategoria(Code, Nombre, Categoria_Id)
	VALUES('IMP', 'Impositiva', @idCategoria)

	INSERT INTO SubCategoria(Code, Nombre, Categoria_Id)
	VALUES('INF', 'Informe comercial', @idCategoria)

	INSERT INTO SubCategoria(Code, Nombre, Categoria_Id)
	VALUES('CAP', 'Carta Presentacion', @idCategoria)
END

COMMIT TRAN
