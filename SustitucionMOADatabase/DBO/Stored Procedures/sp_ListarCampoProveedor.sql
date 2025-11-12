CREATE PROCEDURE [dbo].[sp_ListarCampoProveedor]
  @Ids NVARCHAR(MAX) = NULL,      -- lista de IDs separados por coma, ej: '1,2,3'
    @Cuits NVARCHAR(MAX) = NULL     -- lista de CUITs separados por coma, ej: '20304050607,30405060708'
AS
BEGIN
    SET NOCOUNT ON;

    -- Convertimos las listas en tablas temporales
    DECLARE @TablaIds TABLE (Id INT);
    DECLARE @TablaCuits TABLE (CUIT NVARCHAR(20));

    IF (@Ids IS NOT NULL AND @Ids <> '')
        INSERT INTO @TablaIds (Id)
        SELECT TRY_CAST(value AS INT)
        FROM STRING_SPLIT(@Ids, ',')
        WHERE TRY_CAST(value AS INT) IS NOT NULL;

    IF (@Cuits IS NOT NULL AND @Cuits <> '')
        INSERT INTO @TablaCuits (CUIT)
        SELECT LTRIM(RTRIM(value))
        FROM STRING_SPLIT(@Cuits, ',')
        WHERE LTRIM(RTRIM(value)) <> '';

    SELECT
        cs.IdScato,
        co.Nombre AS NombreCosecha,
        cp.HectareasSoja,
        cp.HectareasTotales,
        cst.Nombre AS NombreCampo,
        cnn.ToneladasAprobadas,
        cp.CampoCosecha_Id,
        p.Id AS IdProveedor,
        p.CodigoProveedor,
        p.RazonSocial AS RazonSocialProveedor,
        p.CUIT AS CUITProveedor,
        cp.CUIT AS CUITCampoProveedor,
        cp.RazonSocial AS RazonSocialCampoProveedor,
        co.Id AS CosechaId,
        cnn.MotivoRechazo,
        cp.FechaCreacion,
        tn.Descripcion AS TipoNormativa,
        tn.Id AS TipoNormativaId,
        cnn.Validado,
        cnn.ValidadoPor,
        cp.EvidenciaEPA_Id,
        cst.Renspa
    FROM CampoProveedor cp
        INNER JOIN CampoCosecha cc ON cp.CampoCosecha_Id = cc.Id
        INNER JOIN CampoSustentable cs ON cc.CampoSustentable_Id = cs.Id
        INNER JOIN Cosecha co ON cc.Cosecha_Id = co.Id
        INNER JOIN Proveedor p ON cp.Proveedor_Id = p.Id
        INNER JOIN CampoCosechaNormativa cnn ON cc.Id = cnn.CampoCosecha_Id
        INNER JOIN TipoNormativa tn ON cnn.TipoNormativa_Id = tn.Id
        INNER JOIN CampoSustentable cst ON cc.CampoSustentable_Id = cst.Id
    WHERE
        (
            (cp.BSVS2 = 1 AND tn.Descripcion = '2BSVS')
            OR (cp.EPA = 1 AND tn.Descripcion = 'EPA')
            OR (cp.EUDR = 1 AND tn.Descripcion = 'EUDR')
        )
        AND cp.Borrado = 0
        AND (
            NOT EXISTS (SELECT 1 FROM @TablaIds)
            OR p.Id IN (SELECT Id FROM @TablaIds)
            OR cp.Proveedor_Id IN (SELECT Id FROM @TablaIds)
        )
        AND (
            NOT EXISTS (SELECT 1 FROM @TablaCuits)
            OR cp.CUIT IN (SELECT CUIT FROM @TablaCuits)
        )
    ORDER BY cp.FechaCreacion DESC;
END