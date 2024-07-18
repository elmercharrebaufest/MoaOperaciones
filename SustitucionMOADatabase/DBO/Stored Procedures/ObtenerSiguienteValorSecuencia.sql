CREATE PROCEDURE ObtenerSiguienteValorSecuencia
AS
BEGIN
    SELECT NEXT VALUE FOR SecuenciaNumeroTemporal AS SiguienteValor;
END;
