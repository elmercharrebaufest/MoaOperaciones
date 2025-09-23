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