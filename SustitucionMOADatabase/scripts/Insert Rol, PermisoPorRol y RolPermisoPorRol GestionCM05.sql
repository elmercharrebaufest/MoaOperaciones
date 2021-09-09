DECLARE @IdRol TABLE (idRol INT)

INSERT Rol (Codigo, Nombre, EsEditable)
OUTPUT INSERTED.Id INTO @IdRol
VALUES ('ADMCM05', 'Gestion CM05', 1)

DECLARE @IdPermisoPorRol TABLE (idPermisoPorRol INT)

INSERT PermisoPorRol (Permiso) 
OUTPUT INSERTED.Id INTO @IdPermisoPorRol
VALUES ('GESTION IMPUESTOS CM05')

INSERT RolPermisoPorRol (Rol_Id, PermisoPorRol_Id) VALUES ((SELECT TOP 1 idRol FROM @IdRol), (SELECT TOP 1 idPermisoPorRol FROM @IdPermisoPorRol))