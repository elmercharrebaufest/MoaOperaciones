BEGIN TRAN

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.PermisoPorRol WHERE Permiso = 'ALTA INTERNA GRANOS')
BEGIN

	INSERT INTO	dbo.PermisoPorRol
	(
	    Permiso
	)
	VALUES
	(
		'ALTA INTERNA GRANOS' -- Permiso - nvarchar(max)
	)

	DECLARE @permisoId INT = SCOPE_IDENTITY()

    insert into Rol VALUES('AIGRAN', 'ALTA INTERNA GRANOS', 1)

	INSERT INTO	dbo.RolPermisoPorRol
	(
	    Rol_Id,
	    PermisoPorRol_Id
	)
	VALUES
	(   (SELECT Id FROM dbo.Rol WHERE Codigo = 'AIGRAN'), -- Rol_Id - int
	    @permisoId  -- PermisoPorRol_Id - int
	)

	/*e agrega al comercial el nuevo permiso de acceso a altas granos*/
	INSERT INTO	dbo.RolPermisoPorRol
	(
	    Rol_Id,
	    PermisoPorRol_Id
	)
	VALUES
	(   (SELECT Id FROM dbo.Rol WHERE Codigo = 'AIGRAN'), -- Rol_Id - int
	    ( SELECT  [Id] [Permiso] FROM [MOAOperaciones].[dbo].[PermisoPorRol] where Permiso = 'ALTA EMPRESA GRANOS')  -- PermisoPorRol_Id - int
	)
END

commit TRAN