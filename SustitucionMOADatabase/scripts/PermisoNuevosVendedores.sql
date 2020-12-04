DECLARE @PermisoId INT;
DECLARE @table table (id int)

IF NOT EXISTS
(
    SELECT TOP (1)
           1
    FROM dbo.PermisoPorRol
    WHERE Permiso = 'NUEVO VENDEDOR'
)
BEGIN
    INSERT INTO dbo.PermisoPorRol
    (
        Permiso
    )
	OUTPUT inserted.Id INTO @table
    VALUES
    (N'NUEVO VENDEDOR' -- Permiso - nvarchar(max)
	);
END;

SELECT @PermisoId = id from @table
--Agrego el nuevo permiso a todos los roles habilitados a ver el menu
BEGIN TRANSACTION
	INSERT INTO dbo.RolPermisoPorRol Values ((SELECT Id FROM dbo.Rol where Codigo = 'MF'), @PermisoId)
	INSERT INTO dbo.RolPermisoPorRol Values ((SELECT Id FROM dbo.Rol where Codigo = 'CORR'), @PermisoId) 
	INSERT INTO dbo.RolPermisoPorRol Values ((SELECT Id FROM dbo.Rol where Codigo = 'NUECORR'), @PermisoId) 
COMMIT TRANSACTION

