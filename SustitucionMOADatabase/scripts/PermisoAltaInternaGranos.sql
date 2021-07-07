
BEGIN TRAN

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.PermisoPorRol WHERE Permiso = 'NOTIFICAR ALTA INTERNA GRANOS')
BEGIN

	INSERT INTO	dbo.PermisoPorRol
	(
	    Permiso
	)
	VALUES
	(
		N'NOTIFICAR ALTA INTERNA GRANOS' -- Permiso - nvarchar(max)
	)


	DECLARE @permisoId INT = SCOPE_IDENTITY()


	INSERT INTO	dbo.RolPermisoPorRol
	(
	    Rol_Id,
	    PermisoPorRol_Id
	)
	VALUES
	(   (SELECT Id FROM dbo.Rol WHERE Codigo = 'COMERCIAL'), -- Rol_Id - int
	    @permisoId  -- PermisoPorRol_Id - int
	)

	INSERT INTO	dbo.RolPermisoPorRol
	(
	    Rol_Id,
	    PermisoPorRol_Id
	)
	VALUES
	(   (SELECT Id FROM dbo.Rol WHERE Codigo = 'MF'), -- Rol_Id - int
	    @permisoId  -- PermisoPorRol_Id - int
	)

	INSERT INTO	dbo.RolPermisoPorRol
	(
	    Rol_Id,
	    PermisoPorRol_Id
	)
	VALUES
	(   (SELECT Id FROM dbo.Rol WHERE Codigo = 'CORR'), -- Rol_Id - int
	    @permisoId  -- PermisoPorRol_Id - int
	)



	INSERT INTO	dbo.RolPermisoPorRol
	(
	    Rol_Id,
	    PermisoPorRol_Id
	)
	VALUES
	(   (SELECT Id FROM dbo.Rol WHERE Codigo = 'NUEG'), -- Rol_Id - int
	    @permisoId  -- PermisoPorRol_Id - int
	    )

		INSERT INTO	dbo.RolPermisoPorRol
	(
	    Rol_Id,
	    PermisoPorRol_Id
	)
	VALUES
	(   (SELECT Id FROM dbo.Rol WHERE Codigo = 'NUECORR'), -- Rol_Id - int
	    @permisoId  -- PermisoPorRol_Id - int
	    )


	/*e agrega al comercial el nuevo permiso de acceso a altas granos*/
	INSERT INTO	dbo.RolPermisoPorRol
	(
	    Rol_Id,
	    PermisoPorRol_Id
	)
	VALUES
	(   (SELECT Id FROM dbo.Rol WHERE Codigo = 'COMERCIAL'), -- Rol_Id - int
	    ( SELECT  [Id] [Permiso] FROM [MOAOperaciones].[dbo].[PermisoPorRol] where Permiso = 'ALTA EMPRESA GRANOS')  -- PermisoPorRol_Id - int
	)
END

COMMIT TRAN