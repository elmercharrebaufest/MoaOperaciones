/* https://baufest.atlassian.net/browse/ARMOA003-3423
 * Actualizar la base donde tenga TieneVisitaObra a TieneVisitaObraMasiva
 */

UPDATE Pliego
SET TieneVisitaObraMasiva = 1
WHERE TieneVisitaObra = 1;
