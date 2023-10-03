-- Llenado de tabla NotificacionPrioridad
BEGIN TRANSACTION;

BEGIN TRY
    INSERT INTO dbo.NotificacionPrioridad (Prioridad_Id, Prioridad_Descripcion)
    SELECT 1, 'ALTA'
    WHERE NOT EXISTS (SELECT 1 FROM dbo.NotificacionPrioridad WHERE Prioridad_Id = 1);

    INSERT INTO dbo.NotificacionPrioridad (Prioridad_Id, Prioridad_Descripcion)
    SELECT 2, 'MEDIA'
    WHERE NOT EXISTS (SELECT 1 FROM dbo.NotificacionPrioridad WHERE Prioridad_Id = 2);

    INSERT INTO dbo.NotificacionPrioridad (Prioridad_Id, Prioridad_Descripcion)
    SELECT 3, 'BAJA'
    WHERE NOT EXISTS (SELECT 1 FROM dbo.NotificacionPrioridad WHERE Prioridad_Id = 3);

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    THROW;
END CATCH;