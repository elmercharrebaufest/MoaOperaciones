
DECLARE @ValoresDistanciaDomicilioReemplazos as TABLE
	(Anterior varchar(100), Nuevo varchar(100))

INSERT INTO @ValoresDistanciaDomicilioReemplazos
VALUES
	('CAP.FEDERAL',				'CAPITAL FEDERAL'),
	('SGO.DEL ESTERO',			'SANTIAGO DEL ESTERO'),
	('TIER.DEL FUEGO',			'TIERRA DEL FUEGO')

INSERT INTO DistanciaDomicilioReemplazos
	(Anterior, Nuevo)
SELECT
	V.Anterior, V.Nuevo
FROM
	@ValoresDistanciaDomicilioReemplazos V left join
	DistanciaDomicilioReemplazos R on V.Anterior = R.Anterior
WHERE R.Id is null
