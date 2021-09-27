IF NOT EXISTS (SELECT TOP 1 1 FROM PermisoPorRol WHERE Permiso = 'BORRAR CAMPOS CREADOS') 
BEGIN 
	INSERT PermisoPorRol (Permiso) VALUES ('BORRAR CAMPOS CREADOS') 
END

DECLARE @PermisoPorRol_Id INT = 
	(SELECT TOP 1 Id FROM PermisoPorRol WHERE Permiso = 'BORRAR CAMPOS CREADOS')
DECLARE @Rol_Id INT = 
	(SELECT TOP 1 Id FROM Rol WHERE Codigo = 'ADMINCCSS')

IF NOT EXISTS (SELECT TOP 1 1 FROM RolPermisoPorRol WHERE Rol_Id = @Rol_Id AND PermisoPorRol_Id = @PermisoPorRol_Id)
BEGIN 
	INSERT RolPermisoPorRol (Rol_Id, PermisoPorRol_Id) VALUES (@Rol_Id, @PermisoPorRol_Id) 
END