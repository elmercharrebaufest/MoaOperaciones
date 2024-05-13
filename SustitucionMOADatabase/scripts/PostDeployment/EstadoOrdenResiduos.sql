
DECLARE @ValoresEstadoOrdenResiduos as TABLE
	(Id int, Nombre varchar(40), NombreExterno varchar(40), Semaforo varchar(15))

INSERT INTO @ValoresEstadoOrdenResiduos
VALUES
	(1,	'Orden generada',		'OK',					'green'),
	(2,	'Pendiente',			'En proceso',			'yellow'),
	(3, 'Orden vencida',		'Orden vencida',		'red'),
	(4, 'Orden entregada',		'Orden entregada',		'white'),
	(5, 'Anulada',				'Anulada',				'red'),
	(6, 'Edición solicitada',	'Edición solicitada',	'yellow'),
	(7, 'Edición rechazada',	'Edición rechazada',	''),
	(8, 'Anulación solicitada',	'Anulación solicitada',	'yellow'),
	(9, 'Ingresada',			'Ingresada',			'green'),
	(10, 'Retirada',			'Retirada',				'green'),
	(11, 'Rechazada',			'Rechazada',			'red')

UPDATE EstadoOrdenResiduos
	SET Nombre=V.Nombre,NombreExterno=V.NombreExterno, Semaforo=V.Semaforo
FROM 
	EstadoOrdenResiduos E left join
	@ValoresEstadoOrdenResiduos V on V.Id = E.Id
WHERE E.Id is not null

INSERT INTO EstadoOrdenResiduos
	(Id, Nombre, NombreExterno, Semaforo)
SELECT
	V.Id, V.Nombre, V.NombreExterno, V.Semaforo
FROM
	@ValoresEstadoOrdenResiduos V left join
	EstadoOrdenResiduos E on V.Id = E.Id
WHERE E.Id is null
