--Obtengo el ID del permiso para contacto
DECLARE @PermisoMailId int;
SELECT @PermisoMailId = Id FROM dbo.PermisoPorRol WHERE Permiso = 'CONTACTO MAIL'

--Actualizo roles de deshabilitados o pendientes de aceptacion para acceso a contacto
BEGIN TRANSACTION
INSERT INTO dbo.RolPermisoPorRol VALUES ((SELECT Id FROM dbo.Rol WHERE Nombre = 'NUEVO CORREDOR'),@PermisoMailId)
INSERT INTO dbo.RolPermisoPorRol VALUES ((SELECT Id FROM dbo.Rol WHERE Nombre = 'NUEVO USUARIO GRANOS'),@PermisoMailId)
INSERT INTO dbo.RolPermisoPorRol VALUES ((SELECT Id FROM dbo.Rol WHERE Nombre = 'USUARIO NO IMPLEMENTADO'),@PermisoMailId)
INSERT INTO dbo.RolPermisoPorRol VALUES ((SELECT Id FROM dbo.Rol WHERE Nombre = 'DESHABILITADO EN DATAAGRO'),@PermisoMailId)
COMMIT TRANSACTION