
-- Verificar si la columna 'NombreColumna' existe en la tabla 'Solp'
IF NOT EXISTS (
    SELECT 1
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'Solp'
      AND COLUMN_NAME = 'SeraUsadoEnPliegoMultiple'
)
BEGIN
    -- Crear la columna si no existe
    ALTER TABLE dbo.Solp
    ADD SeraUsadoEnPliegoMultiple BIT NOT NULL DEFAULT 0; -- Cambia el tipo de datos según sea necesario
END
