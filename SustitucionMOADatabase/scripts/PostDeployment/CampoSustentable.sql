-- Insercion tipos de normativas EPA, EUDER, BSVS2
IF NOT EXISTS (SELECT 1 FROM [dbo].[TipoNormativa] WHERE Descripcion = 'BSVS2')
BEGIN
    INSERT INTO [dbo].[TipoNormativa] (Descripcion)
    VALUES ('BSVS2')
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[TipoNormativa] WHERE Descripcion = 'EPA')
BEGIN
    INSERT INTO [dbo].[TipoNormativa] (Descripcion)
    VALUES ('EPA')
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[TipoNormativa] WHERE Descripcion = 'EUDER')
BEGIN
    INSERT INTO [dbo].[TipoNormativa] (Descripcion)
    VALUES ('EUDER')
END