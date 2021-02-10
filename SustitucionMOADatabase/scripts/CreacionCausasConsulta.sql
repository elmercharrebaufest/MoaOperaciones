SET NOCOUNT ON
BEGIN TRAN

IF NOT EXISTS(SELECT 1 FROM CausaConsulta WHERE Nombre = 'Sin Exclusión')
BEGIN
	INSERT INTO CausaConsulta(Nombre)
	VALUES ('Sin Exclusión')
END

IF NOT EXISTS(SELECT 1 FROM CausaConsulta WHERE Nombre = 'Jurisdicción Errónea')
BEGIN
	INSERT INTO CausaConsulta(Nombre)
	VALUES ('Jurisdicción Errónea')
END

IF NOT EXISTS(SELECT 1 FROM CausaConsulta WHERE Nombre = 'Jurisdicción Errónea OC')
BEGIN
	INSERT INTO CausaConsulta(Nombre)
	VALUES ('Jurisdicción Errónea OC')
END

IF NOT EXISTS(SELECT 1 FROM CausaConsulta WHERE Nombre = 'Diferencia alícuota')
BEGIN
	INSERT INTO CausaConsulta(Nombre)
	VALUES ('Diferencia alícuota')
END

IF NOT EXISTS(SELECT 1 FROM CausaConsulta WHERE Nombre = 'Diferencia Base')
BEGIN
	INSERT INTO CausaConsulta(Nombre)
	VALUES ('Diferencia Base')
END

IF NOT EXISTS(SELECT 1 FROM CausaConsulta WHERE Nombre = 'Sistema')
BEGIN
	INSERT INTO CausaConsulta(Nombre)
	VALUES ('Sistema')
END

COMMIT TRAN
