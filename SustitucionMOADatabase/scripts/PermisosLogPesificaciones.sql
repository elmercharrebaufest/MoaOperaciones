
BEGIN TRAN

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.PermisoPorRol WHERE Permiso = 'GUARDADO Y CONSULTA DE LOG PESIFICACIONES')
BEGIN

	INSERT INTO	dbo.PermisoPorRol
	(
	    Permiso
	)
	VALUES
	(
		N'GUARDADO Y CONSULTA DE LOG PESIFICACIONES' -- Permiso - nvarchar(max)
	)


	DECLARE @permisoId INT = SCOPE_IDENTITY()


	INSERT INTO	dbo.RolPermisoPorRol
	(
	    Rol_Id,
	    PermisoPorRol_Id
	)
	VALUES
	(   (SELECT Id FROM dbo.Rol WHERE Codigo = 'ADM'), -- Rol_Id - int
	    @permisoId  -- PermisoPorRol_Id - int
	    )

END

COMMIT TRAN