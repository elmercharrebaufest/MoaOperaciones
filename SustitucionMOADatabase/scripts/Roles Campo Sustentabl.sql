BEGIN TRAN
SET NOCOUNT ON
IF NOT EXISTS (SELECT TOP 1 1 FROM PermisoPorRol WHERE Permiso = 'ABM CAMPOS SUSTENTABLE')
BEGIN 

   INSERT INTO PermisoPorRol
   VALUES ('ABM CAMPOS SUSTENTABLE')

END


IF NOT EXISTS (SELECT TOP 1 1 FROM PermisoPorRol WHERE Permiso = 'VER TODOS CAMPOS SUSTENTABLE')
BEGIN 

   INSERT INTO PermisoPorRol
   VALUES ('VER TODOS CAMPOS SUSTENTABLE')

END

IF NOT EXISTS (SELECT TOP 1 1 FROM PermisoPorRol WHERE Permiso = 'EDICION CAMPOS CREADOS')
BEGIN 

   INSERT INTO PermisoPorRol
   VALUES ('EDICION CAMPOS CREADOS')

END


IF NOT EXISTS (SELECT TOP 1 1 FROM Rol WHERE Nombre = 'ADMIN CAMPO SUSTENTABLE')
BEGIN 

   INSERT INTO Rol (Codigo, Nombre, EsEditable)
   VALUES ('ADMINCCSS', 'ADMIN CAMPO SUSTENTABLE', 1)

END


DECLARE @RolGranosId INT, @RolAdminCampoId INT, @PermisoABMCampoId INT, @PermisoVerCamposId INT, @PermisoEditarCamposId INT


SET @RolGranosId = (SELECT Id FROM Rol WHERE Nombre = 'GRANOS')
SET @RolAdminCampoId = (SELECT Id FROM Rol WHERE Nombre = 'ADMIN CAMPO SUSTENTABLE')


SET @PermisoABMCampoId = (SELECT Id FROM PermisoPorRol WHERE Permiso = 'ABM CAMPOS SUSTENTABLE')
SET @PermisoVerCamposId = (SELECT Id FROM PermisoPorRol WHERE Permiso = 'VER TODOS CAMPOS SUSTENTABLE')
SET @PermisoEditarCamposId = (SELECT Id FROM PermisoPorRol WHERE Permiso = 'EDICION CAMPOS CREADOS')

IF NOT EXISTS( SELECT TOP 1 1 FROM RolPermisoPorRol WHERE Rol_Id = @RolGranosId AND PermisoPorRol_Id = @PermisoABMCampoId)
BEGIN
    INSERT INTO RolPermisoPorRol (Rol_Id, PermisoPorRol_Id)
    VALUES (@RolGranosId, @PermisoABMCampoId)
END 

IF NOT EXISTS( SELECT TOP 1 1 FROM RolPermisoPorRol WHERE Rol_Id = @RolAdminCampoId AND PermisoPorRol_Id = @PermisoVerCamposId)
BEGIN
    INSERT INTO RolPermisoPorRol (Rol_Id, PermisoPorRol_Id)
    VALUES (@RolAdminCampoId, @PermisoVerCamposId)
END 

IF NOT EXISTS( SELECT TOP 1 1 FROM RolPermisoPorRol WHERE Rol_Id = @RolAdminCampoId AND PermisoPorRol_Id = @PermisoEditarCamposId)
BEGIN
    INSERT INTO RolPermisoPorRol (Rol_Id, PermisoPorRol_Id)
    VALUES (@RolAdminCampoId, @PermisoEditarCamposId)
END 


COMMIT TRAN