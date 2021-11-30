IF NOT EXISTS (SELECT TOP 1 1 FROM OrigenMovimientoIngresosBrutosCoeficienteUnificado WHERE Id = 1 AND Descripcion = 'WEB') 
BEGIN
    INSERT OrigenMovimientoIngresosBrutosCoeficienteUnificado (Id, Descripcion) VALUES (1, 'WEB')
END

IF NOT EXISTS (SELECT TOP 1 1 FROM OrigenMovimientoIngresosBrutosCoeficienteUnificado WHERE Id = 2 AND Descripcion = 'SAP') 
BEGIN
    INSERT OrigenMovimientoIngresosBrutosCoeficienteUnificado (Id, Descripcion) VALUES (2, 'SAP')
END