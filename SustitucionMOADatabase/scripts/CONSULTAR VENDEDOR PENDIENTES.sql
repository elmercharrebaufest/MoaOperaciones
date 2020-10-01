IF NOT EXISTS
(
    SELECT TOP (1)
           1
    FROM dbo.PermisoPorRol
    WHERE Permiso = 'CONSULTAR VENDEDOR PENDIENTES'
)
BEGIN
    INSERT INTO dbo.PermisoPorRol
    (
        Permiso
    )
    VALUES
    (N'CONSULTAR VENDEDOR PENDIENTES' -- Permiso - nvarchar(max)
        );

	DECLARE @permisoID INT = SCOPE_IDENTITY()

	INSERT INTO dbo.RolPermisoPorRol
	(
	    Rol_Id,
	    PermisoPorRol_Id
	)
	SELECT Id, @permisoID FROM dbo.Rol WHERE Codigo IN('NUECORR', 'CORR')
	
END;