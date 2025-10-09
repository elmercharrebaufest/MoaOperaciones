-- Insercion tipos de normativas EPA, EUDR, 2BSVS
IF NOT EXISTS (SELECT 1 FROM [dbo].[TipoNormativa] WHERE Descripcion = '2BSVS')
BEGIN
    INSERT INTO [dbo].[TipoNormativa] (Descripcion)
    VALUES ('2BSVS')
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[TipoNormativa] WHERE Descripcion = 'EPA')
BEGIN
    INSERT INTO [dbo].[TipoNormativa] (Descripcion)
    VALUES ('EPA')
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[TipoNormativa] WHERE Descripcion = 'EUDR')
BEGIN
    INSERT INTO [dbo].[TipoNormativa] (Descripcion)
    VALUES ('EUDR')
END

-- Actualiza el campo DirectivaDDJJCampoSustentable a 'ISO 2' para las cosechas indicadas si está en NULL
UPDATE [dbo].[Cosecha]
SET DirectivaDDJJCampoSustentable = '2023/2413/EC (RED III)'
WHERE Nombre IN ('20-21', '21-22', '22-23', '23-24', '24-25')
AND DirectivaDDJJCampoSustentable IS NULL;