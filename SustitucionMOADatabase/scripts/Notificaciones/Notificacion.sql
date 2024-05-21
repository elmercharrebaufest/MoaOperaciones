-- HU04 - Colocar prioridad 2 (MEDIA) si el contenido del campo prioridad es NULL
BEGIN TRY
    -- Iniciar la transacción
    BEGIN TRANSACTION;

    -- Colocar prioridad 2 (MEDIA) si el contenido del campo prioridad es NULL
    UPDATE Notificacion SET Prioridad = ISNULL(Prioridad, 2);

    -- Confirmar la transacción si no hay errores
    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    -- Si ocurre un error, realizamos ROLLBACK
    ROLLBACK TRANSACTION;
    THROW;
END CATCH;





-- HU05 - Si el campo Descripcion es VARCHAR lo cambia a NVARCAHAR para que permita caracteres Unicode.
BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @Tabla NVARCHAR(128) = 'Notificacion';
    DECLARE @Columna NVARCHAR(128) = 'Mensaje';

    -- Verificar si el campo es VARCHAR(MAX)
    IF EXISTS (
        SELECT *
        FROM INFORMATION_SCHEMA.COLUMNS
        WHERE TABLE_NAME = @Tabla
        AND COLUMN_NAME = @Columna
        AND DATA_TYPE = 'varchar'
        AND CHARACTER_MAXIMUM_LENGTH = -1 -- El valor -1 indica que es VARCHAR(MAX)
    )
    BEGIN
        -- Realizar el cambio de tipo a NVARCHAR(MAX)
        ALTER TABLE Notificacion
        ALTER COLUMN Mensaje NVARCHAR(MAX);
        
        -- Confirmar la transacción si no hay errores
        COMMIT TRANSACTION;
    END
    ELSE
    BEGIN
        -- Si el campo no es VARCHAR(MAX), no hacer nada y realizar rollback
        ROLLBACK TRANSACTION;
    END;
END TRY
BEGIN CATCH
    -- Si ocurre un error, realizamos ROLLBACK para las sentencias DDL
    ROLLBACK TRANSACTION;
    THROW;
END CATCH;
