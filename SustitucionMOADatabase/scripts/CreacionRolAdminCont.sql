SET NOCOUNT ON
BEGIN TRAN

/*
Permisos:
ADMIN CONTABILIZACION MES ANTERIOR

Rol: 
ADMINCONTMA
*/

DECLARE @rolIdAdminContMA INT = (SELECT TOP 1 Id FROM dbo.Rol WHERE Nombre = 'ADMIN CONTABILIZACION MES ANTERIOR')
DECLARE @permisoID INT = 0

IF (@rolIdAdminContMA IS NULL)
BEGIN
	INSERT INTO	dbo.Rol
	(
	    Codigo,
	    Nombre,
	    EsEditable
	)
	VALUES
	(   N'ADMINCONTMA', -- Codigo - nvarchar(max)
	    N'ADMIN CONTABILIZACION MES ANTERIOR', -- Nombre - nvarchar(max)
	    1 -- EsEditable - bit
	)

	SET @rolIdAdminContMA = SCOPE_IDENTITY()

END


--ADMIN CONTABILIZACION MES ANTERIOR
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.PermisoPorRol WHERE Permiso = 'ADMIN CONTABILIZACION MES ANTERIOR')
BEGIN
	INSERT INTO dbo.PermisoPorRol
	(
	    Permiso
	)
	VALUES
	(N'ADMIN CONTABILIZACION MES ANTERIOR' -- Permiso - nvarchar(max)
	)

	SET @permisoID = SCOPE_IDENTITY()

	INSERT INTO dbo.RolPermisoPorRol
	(
	    Rol_Id,
	    PermisoPorRol_Id
	)
	VALUES
	(   @rolIdAdminContMA, -- Rol_Id - int
	    @permisoID  -- PermisoPorRol_Id - int
	    )	
END

COMMIT TRAN