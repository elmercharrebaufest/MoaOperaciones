SET NOCOUNT ON
BEGIN TRAN

/*
Permisos:
Alta SOLP

Rol: 
SOLP
*/

DECLARE @rolIdSolp INT = (SELECT TOP 1 Id FROM dbo.Rol WHERE Nombre = 'SOLP')
DECLARE @rolIdAdmin INT = (SELECT TOP 1 Id FROM dbo.Rol WHERE Nombre = 'ADMINISTRACION')
DECLARE @permisoID INT = 0

IF (@rolIdSolp IS NULL)
BEGIN
	INSERT INTO	dbo.Rol
	(
	    Codigo,
	    Nombre,
	    EsEditable
	)
	VALUES
	(   N'SOLP', -- Codigo - nvarchar(max)
	    N'SOLP', -- Nombre - nvarchar(max)
	    1 -- EsEditable - bit
	)

	SET @rolIdSolp = SCOPE_IDENTITY()

END


--ABM SOLP
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.PermisoPorRol WHERE Permiso = 'ABM SOLP')
BEGIN
	INSERT INTO dbo.PermisoPorRol
	(
	    Permiso
	)
	VALUES
	(N'ABM SOLP' -- Permiso - nvarchar(max)
	)

	SET @permisoID = SCOPE_IDENTITY()

	INSERT INTO dbo.RolPermisoPorRol
	(
	    Rol_Id,
	    PermisoPorRol_Id
	)
	VALUES
	(   @rolIdAdmin, -- Rol_Id - int
	    @permisoID  -- PermisoPorRol_Id - int
	    )
	
	INSERT INTO dbo.RolPermisoPorRol
	(
	    Rol_Id,
	    PermisoPorRol_Id
	)
	VALUES
	(   @rolIdSolp, -- Rol_Id - int
	    @permisoID  -- PermisoPorRol_Id - int
	    )
END

COMMIT TRAN