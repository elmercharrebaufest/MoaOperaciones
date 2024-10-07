UPDATE OrdenDeCargaFason
SET ApellidoChofer= CASE 
              WHEN CHARINDEX(' ', NombreChofer) > 0 
              THEN LEFT(NombreChofer, CHARINDEX(' ', NombreChofer) - 1)
              ELSE NombreChofer
           END,
    NombreChofer= CASE 
                   WHEN CHARINDEX(' ', NombreChofer) > 0 
                   THEN SUBSTRING(NombreChofer, CHARINDEX(' ', NombreChofer) + 1, LEN(NombreChofer))
                   ELSE NombreChofer
                END
WHERE ApellidoChofer='DEFAULT_ONLY_ONE_USE';