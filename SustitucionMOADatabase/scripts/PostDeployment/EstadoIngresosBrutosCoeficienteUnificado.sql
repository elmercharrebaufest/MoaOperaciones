IF NOT EXISTS (SELECT TOP 1 1 FROM EstadoIngresosBrutosCoeficienteUnificado WHERE Id = 1 AND Descripcion = 'Pendiente') 
BEGIN 
	INSERT EstadoIngresosBrutosCoeficienteUnificado (Id, Descripcion) VALUES (1, 'Pendiente') 
END

IF NOT EXISTS (SELECT TOP 1 1 FROM EstadoIngresosBrutosCoeficienteUnificado WHERE Id = 2 AND Descripcion = 'Autorizado') 
BEGIN 
	INSERT EstadoIngresosBrutosCoeficienteUnificado (Id, Descripcion) VALUES (2, 'Autorizado') 
END

IF NOT EXISTS (SELECT TOP 1 1 FROM EstadoIngresosBrutosCoeficienteUnificado WHERE Id = 3 AND Descripcion = 'Completado') 
BEGIN 
	INSERT EstadoIngresosBrutosCoeficienteUnificado (Id, Descripcion) VALUES (3, 'Completado') 
END

IF NOT EXISTS (SELECT TOP 1 1 FROM EstadoIngresosBrutosCoeficienteUnificado WHERE Id = 4 AND Descripcion = 'Rechazado por usuario') 
BEGIN 
	INSERT EstadoIngresosBrutosCoeficienteUnificado (Id, Descripcion) VALUES (4, 'Rechazado por usuario') 
END

IF NOT EXISTS (SELECT TOP 1 1 FROM EstadoIngresosBrutosCoeficienteUnificado WHERE Id = 5 AND Descripcion = 'Rechazado por sistema') 
BEGIN 
	INSERT EstadoIngresosBrutosCoeficienteUnificado (Id, Descripcion) VALUES (5, 'Rechazado por sistema') 
END