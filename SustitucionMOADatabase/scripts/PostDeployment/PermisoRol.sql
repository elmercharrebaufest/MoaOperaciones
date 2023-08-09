DECLARE @PermisoId INT;
BEGIN TRAN

IF NOT EXISTS (SELECT TOP 1 1 
               FROM dbo.PermisoPorRol 
               WHERE Permiso = 'ANULAR ORDEN DE CARGA')
BEGIN
    INSERT INTO    dbo.PermisoPorRol(Permiso) VALUES(N'ANULAR ORDEN DE CARGA')
    SET @PermisoId = SCOPE_IDENTITY();

    INSERT INTO    dbo.RolPermisoPorRol(Rol_Id, PermisoPorRol_Id)
    VALUES((SELECT Id FROM dbo.Rol WHERE Codigo = 'ANUL'), @PermisoId)
END


IF NOT EXISTS(SELECT TOP (1) 1
              FROM dbo.PermisoPorRol
              WHERE Permiso = 'ENVIAR A SAP')
BEGIN
    INSERT INTO    dbo.PermisoPorRol(Permiso) VALUES(N'ENVIAR A SAP')
    SET @PermisoId = SCOPE_IDENTITY();

    IF NOT EXISTS(SELECT TOP (1) 1
                  FROM dbo.RolPermisoPorRol
                  WHERE Rol_Id IN (SELECT Id FROM dbo.Rol WHERE Codigo = 'ADM') 
                  AND PermisoPorRol_Id = @PermisoId)
    BEGIN 
        INSERT INTO dbo.RolPermisoPorRol VALUES ((SELECT Id FROM dbo.Rol WHERE Codigo = 'ADM'), @PermisoId)
    END

    IF NOT EXISTS(SELECT TOP (1) 1 
                  FROM dbo.RolPermisoPorRol
                  WHERE Rol_Id IN (SELECT Id FROM dbo.Rol WHERE Codigo = 'ANUL') 
                  AND PermisoPorRol_Id = @PermisoId)
    BEGIN 
        INSERT INTO dbo.RolPermisoPorRol Values ((SELECT Id FROM dbo.Rol WHERE Codigo = 'ANUL'), @PermisoId)
    END
END


IF NOT EXISTS(SELECT TOP (1) 1
              FROM dbo.PermisoPorRol
              WHERE Permiso = 'VER SOLPS COMPRADOR')
BEGIN
    INSERT INTO dbo.PermisoPorRol(Permiso)
    VALUES (N'VER SOLPS COMPRADOR')

    SET @PermisoId = SCOPE_IDENTITY();

	IF NOT EXISTS(SELECT TOP (1) 1 
                  FROM dbo.RolPermisoPorRol
                  WHERE Rol_Id IN (SELECT Id FROM dbo.Rol WHERE Codigo = 'COMPRADOR') 
                  AND PermisoPorRol_Id = @PermisoId)
    BEGIN 
        INSERT INTO dbo.RolPermisoPorRol Values ((SELECT Id FROM dbo.Rol WHERE Codigo = 'COMPRADOR'), @PermisoId)
    END
END;

COMMIT TRAN