SET NOCOUNT ON;
BEGIN TRAN;
DECLARE @rolId INT = 0;

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
END;

IF NOT EXISTS (SELECT TOP (1) 1 FROM dbo.Rol WHERE Nombre = 'NUEVOCORR')
BEGIN
    INSERT INTO dbo.Rol
    (
        Codigo,
        Nombre
    )
    VALUES
    (   N'NUECORR',       -- Codigo - nvarchar(max)
        N'NUEVO CORREDOR' -- Nombre - nvarchar(max)
        );

    SET @rolId = SCOPE_IDENTITY();

    INSERT INTO dbo.RolPermisoPorRol
    (
        Rol_Id,
        PermisoPorRol_Id
    )
    SELECT @rolId,
           Id
    FROM dbo.PermisoPorRol
    WHERE Permiso IN ( 'CONSULTAR DATOS FISCALES', 'CONSULTAR VENDEDORES', 'CONSULTAR VENDEDOR PENDIENTES',
                       'CONSULTAR DOCUMENTACION', 'ALTA EMPRESA GRANOS'
                     );
END;

IF NOT EXISTS (SELECT TOP (1) 1 FROM dbo.Rol WHERE Nombre = 'CORR')
BEGIN
    INSERT INTO dbo.Rol
    (
        Codigo,
        Nombre
    )
    VALUES
    (   N'CORR',    -- Codigo - nvarchar(max)
        N'CORREDOR' -- Nombre - nvarchar(max)
        );

    SET @rolId = SCOPE_IDENTITY();
    INSERT INTO dbo.RolPermisoPorRol
    (
        Rol_Id,
        PermisoPorRol_Id
    )
    SELECT @rolId,
           Id
    FROM dbo.PermisoPorRol
    WHERE Permiso IN ( 'CONSULTAR DATOS FISCALES', 'CONSULTAR VENDEDORES', 'CONSULTAR VENDEDOR PENDIENTES',
                       'CONSULTAR DOCUMENTACION', 'ALTA EMPRESA GRANOS'
                     );
END;

COMMIT TRAN;