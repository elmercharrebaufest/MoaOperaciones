IF NOT EXISTS (SELECT TOP 1 1 FROM TipoImputacionSAP WHERE Descripcion = 'centroDeCosto') 
BEGIN
    INSERT TipoImputacionSAP(Descripcion, Codigo, TablaGeneral_Id) VALUES ('centroDeCosto', 'K', 11)
END

IF NOT EXISTS (SELECT TOP 1 1 FROM TipoImputacionSAP WHERE Descripcion = 'ordenDeOt') 
BEGIN
    INSERT TipoImputacionSAP(Descripcion, Codigo, TablaGeneral_Id) VALUES ('ordenDeOt', 'F', 12)
END

IF NOT EXISTS (SELECT TOP 1 1 FROM TipoImputacionSAP WHERE Descripcion = 'siniestroBeneficio') 
BEGIN
    INSERT TipoImputacionSAP(Descripcion, Codigo, TablaGeneral_Id) VALUES ('siniestroBeneficio', 'Y', 14)
END