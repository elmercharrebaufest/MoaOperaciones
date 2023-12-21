-- Llenado FechaCreacion para Novedades previas.
BEGIN TRY
    BEGIN TRANSACTION;

    UPDATE Notificacion
    SET FechaCreacion = FechaInicio
    WHERE FechaCreacion IS NULL;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
END CATCH;
