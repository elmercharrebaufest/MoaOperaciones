/* https://baufest.atlassian.net/browse/ARMOA003-3423
 * Actualizar la base donde tenga TieneVisitaObra a TieneVisitaObraMasiva
 */

IF EXISTS(SELECT 1 FROM sys.columns WHERE Name = N'TieneVisitaObra' AND Object_ID = Object_ID(N'dbo.Pliego'))
BEGIN

	EXEC ('
		UPDATE Pliego
		SET TieneVisitaObraMasiva = 1
		WHERE TieneVisitaObra = 1;')

	UPDATE Pliego
	SET TieneVisitaObraMasiva = 0
	WHERE TieneVisitaObraMasiva IS NULL;

END
