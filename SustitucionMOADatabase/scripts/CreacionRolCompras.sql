SET NOCOUNT ON
BEGIN TRAN

/*
Permisos:
ABM EMPRESAS COMPRAS
VER ALTAS GRANOS
VER ALTAS NO GRANOS

Rol: 
COMPRAS
*/

DECLARE @rolIdCompras INT = (SELECT TOP 1 Id FROM dbo.Rol WHERE Nombre = 'COMPRAS')
DECLARE @rolIdAdmin INT = (SELECT TOP 1 Id FROM dbo.Rol WHERE Nombre = 'ADMINISTRACION')
DECLARE @rolIdOperador INT = (SELECT TOP 1 Id FROM dbo.Rol WHERE Nombre = 'OPERADOR')
DECLARE @rolIdAprobador INT = (SELECT TOP 1 Id FROM dbo.Rol WHERE Nombre = 'APROBADOR')
DECLARE @permisoID INT = 0



IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.Rol WHERE Nombre = 'COMPRAS')
BEGIN
	INSERT INTO	dbo.Rol
	(
	    Codigo,
	    Nombre,
	    EsEditable
	)
	VALUES
	(   N'COMPRAS', -- Codigo - nvarchar(max)
	    N'COMPRAS', -- Nombre - nvarchar(max)
	    1 -- EsEditable - bit
	)

	SET @rolIdCompras = SCOPE_IDENTITY()

	SET @permisoID = (SELECT TOP 1 Id FROM dbo.PermisoPorRol WHERE Permiso = 'ABM EMPRESAS')

	INSERT INTO	 dbo.RolPermisoPorRol
	(
	    Rol_Id,
	    PermisoPorRol_Id
	)
	VALUES
	(   @rolIdCompras, -- Rol_Id - int
	    @permisoID  -- PermisoPorRol_Id - int
	    )

END


--ALTA EMPRESA NO GRANOS
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.PermisoPorRol WHERE Permiso = 'ALTA EMPRESA NO GRANOS')
BEGIN
	INSERT INTO dbo.PermisoPorRol
	(
	    Permiso
	)
	VALUES
	(N'ALTA EMPRESA NO GRANOS' -- Permiso - nvarchar(max)
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
	(   @rolIdCompras, -- Rol_Id - int
	    @permisoID  -- PermisoPorRol_Id - int
	    )
END


--VER ALTAS GRANOS
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.PermisoPorRol WHERE Permiso = 'VER ALTAS GRANOS')
BEGIN
	INSERT INTO dbo.PermisoPorRol
	(
	    Permiso
	)
	VALUES
	(N'VER ALTAS GRANOS' -- Permiso - nvarchar(max)
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
	(   @rolIdAprobador, -- Rol_Id - int
	    @permisoID  -- PermisoPorRol_Id - int
	    )

	INSERT INTO dbo.RolPermisoPorRol
	(
	    Rol_Id,
	    PermisoPorRol_Id
	)
	VALUES
	(   @rolIdOperador, -- Rol_Id - int
	    @permisoID  -- PermisoPorRol_Id - int
	    )
END


--VER ALTAS NO GRANOS
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.PermisoPorRol WHERE Permiso = 'VER ALTAS NO GRANOS')
BEGIN
	INSERT INTO dbo.PermisoPorRol
	(
	    Permiso
	)
	VALUES
	(N'VER ALTAS NO GRANOS' -- Permiso - nvarchar(max)
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
	(   @rolIdAprobador, -- Rol_Id - int
	    @permisoID  -- PermisoPorRol_Id - int
	    )

	INSERT INTO dbo.RolPermisoPorRol
	(
	    Rol_Id,
	    PermisoPorRol_Id
	)
	VALUES
	(   @rolIdOperador, -- Rol_Id - int
	    @permisoID  -- PermisoPorRol_Id - int
	    )

		
	INSERT INTO dbo.RolPermisoPorRol
	(
	    Rol_Id,
	    PermisoPorRol_Id
	)
	VALUES
	(   
		@rolIdCompras, -- Rol_Id - int
	    @permisoID  -- PermisoPorRol_Id - int
	)
END

COMMIT TRAN