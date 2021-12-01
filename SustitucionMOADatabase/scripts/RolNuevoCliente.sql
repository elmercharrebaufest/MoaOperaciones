
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.Rol WHERE  Codigo = 'NUECLI')
BEGIN
	INSERT INTO dbo.Rol
	(
	    Codigo,
	    Nombre,
	    EsEditable
	)
	VALUES
	(   N'NUECLI', -- Codigo - nvarchar(max)
	    N'NUEVO CLIENTE ', -- Nombre - nvarchar(max)
	    1 -- EsEditable - bit
	    )
	DECLARE @rolId INT = SCOPE_IDENTITY()

	--Insertamos permiso de contacto y de estado solicitud
	INSERT INTO dbo.RolPermisoPorRol
	(
	    Rol_Id,
	    PermisoPorRol_Id
	)
	VALUES
	(   @rolId, -- Rol_Id - int
	    (SELECT TOP 1 id FROM dbo.PermisoPorRol WHERE Permiso = 'CONTACTO MAIL')  -- PermisoPorRol_Id - int
	)

	INSERT INTO dbo.RolPermisoPorRol
	(
	    Rol_Id,
	    PermisoPorRol_Id
	)
	VALUES
	(   @rolId, -- Rol_Id - int
	    (SELECT id FROM dbo.PermisoPorRol WHERE Permiso = 'ESTADO SOLICITUD')  -- PermisoPorRol_Id - int
	)
END