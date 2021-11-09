IF NOT EXISTS (SELECT TOP 1 1 FROM Rol WHERE Codigo = 'ADMCM05') 
BEGIN
    INSERT Rol (Codigo, Nombre, EsEditable) VALUES ('ADMCM05', 'Gestion CM05', 1)
END
GO

IF NOT EXISTS (SELECT TOP 1 1 FROM PermisoPorRol WHERE Permiso = 'GESTION IMPUESTOS CM05') 
BEGIN
    INSERT PermisoPorRol (Permiso) VALUES ('GESTION IMPUESTOS CM05')
END
GO

DECLARE @IdRol INT = (SELECT TOP 1 Id FROM Rol WHERE Codigo = 'ADMCM05')
DECLARE @IdPermisoPorRol INT = (SELECT TOP 1 Id FROM PermisoPorRol WHERE Permiso = 'GESTION IMPUESTOS CM05')

IF NOT EXISTS (SELECT TOP 1 1 FROM RolPermisoPorRol WHERE Rol_Id = @IdRol AND PermisoPorRol_Id = @IdPermisoPorRol) 
BEGIN
    INSERT RolPermisoPorRol (Rol_Id, PermisoPorRol_Id) VALUES (@IdRol, @IdPermisoPorRol)
END
GO