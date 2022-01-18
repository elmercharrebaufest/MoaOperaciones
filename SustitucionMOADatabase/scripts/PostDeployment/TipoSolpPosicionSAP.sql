IF NOT EXISTS (SELECT TOP 1 1 FROM TipoSolpPosicionSAP WHERE Descripcion = 'Servicio') 
BEGIN
    INSERT TipoSolpPosicionSAP(Id, Descripcion, Codigo, TablaGeneral_Id) VALUES (1, 'Servicio', '9', 9)
END
