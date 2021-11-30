IF NOT EXISTS (SELECT TOP 1 1 FROM TipoMovimientoIngresosBrutosCoeficienteUnificado WHERE Descripcion = 'Creacion') 
BEGIN
    INSERT TipoMovimientoIngresosBrutosCoeficienteUnificado (Id, Descripcion) VALUES (1, 'Creacion')
END

IF NOT EXISTS (SELECT TOP 1 1 FROM TipoMovimientoIngresosBrutosCoeficienteUnificado WHERE Descripcion = 'Autorizacion') 
BEGIN
    INSERT TipoMovimientoIngresosBrutosCoeficienteUnificado (Id, Descripcion) VALUES (2, 'Autorizacion')
END

IF NOT EXISTS (SELECT TOP 1 1 FROM TipoMovimientoIngresosBrutosCoeficienteUnificado WHERE Descripcion = 'Autorizacion Revertida') 
BEGIN
    INSERT TipoMovimientoIngresosBrutosCoeficienteUnificado (Id, Descripcion) VALUES (3, 'Autorizacion Revertida')
END

IF NOT EXISTS (SELECT TOP 1 1 FROM TipoMovimientoIngresosBrutosCoeficienteUnificado WHERE Descripcion = 'Exportacion Exitosa') 
BEGIN
    INSERT TipoMovimientoIngresosBrutosCoeficienteUnificado (Id, Descripcion) VALUES (4, 'Exportacion Exitosa')
END

IF NOT EXISTS (SELECT TOP 1 1 FROM TipoMovimientoIngresosBrutosCoeficienteUnificado WHERE Descripcion = 'Error') 
BEGIN
    INSERT TipoMovimientoIngresosBrutosCoeficienteUnificado (Id, Descripcion) VALUES (5, 'Error')
END