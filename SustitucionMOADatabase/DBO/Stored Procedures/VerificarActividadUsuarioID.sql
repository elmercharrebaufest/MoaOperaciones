--exec VerificarActividadUsuarioID 14;
CREATE PROCEDURE VerificarActividadUsuarioID (
    @UsuarioID INT
)
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Tabla NVARCHAR(255);
    DECLARE @Columna NVARCHAR(255);
    DECLARE @SQL NVARCHAR(MAX);

	 -- Crear tabla temporal para almacenar los resultados
    CREATE TABLE #Resultados (
        Tabla NVARCHAR(255),
        Columna NVARCHAR(255),
        SeEncontraronRegistros BIT
    );

    -- Consultar las claves foráneas relacionadas con la tabla Usuario
    SELECT 
        OBJECT_NAME(f.parent_object_id) AS Tabla,
        COL_NAME(fc.parent_object_id, fc.parent_column_id) AS Columna
    INTO 
        #FKColumns
    FROM 
        sys.foreign_keys AS f
    INNER JOIN 
        sys.foreign_key_columns AS fc ON f.object_id = fc.constraint_object_id
    WHERE 
        OBJECT_NAME(f.referenced_object_id) = 'Usuario' 
		and OBJECT_NAME(f.parent_object_id) <> 'ProveedorUsuario' and OBJECT_NAME(f.parent_object_id) <> 'RolUsuario'
		and OBJECT_NAME(f.parent_object_id) <> 'UsuarioGranos' and OBJECT_NAME(f.parent_object_id) <> 'UsuarioNoGranos';


    -- Iterar sobre las tablas relacionadas y verificar si existen registros
    DECLARE tabla_cursor CURSOR FOR
    SELECT Tabla, Columna FROM #FKColumns;

    OPEN tabla_cursor;
    FETCH NEXT FROM tabla_cursor INTO @Tabla, @Columna;

    WHILE @@FETCH_STATUS = 0
    BEGIN
         SET @SQL = 'INSERT INTO #Resultados (Tabla, Columna, SeEncontraronRegistros) ' +
                   'SELECT ''' + @Tabla + ''', ''' + @Columna + ''', ' +
                   'CASE WHEN EXISTS (SELECT 1 FROM ' + QUOTENAME(@Tabla) + ' WHERE ' + QUOTENAME(@Columna) + ' = ' + CAST(@UsuarioID AS NVARCHAR(10)) + ') THEN 1 ELSE 0 END';

        EXEC sp_executesql @SQL;

        FETCH NEXT FROM tabla_cursor INTO @Tabla, @Columna;
    END

    CLOSE tabla_cursor;
    DEALLOCATE tabla_cursor;
    DROP TABLE #FKColumns;

	 -- Seleccionar resultados de la tabla temporal
    SELECT * FROM #Resultados;
END