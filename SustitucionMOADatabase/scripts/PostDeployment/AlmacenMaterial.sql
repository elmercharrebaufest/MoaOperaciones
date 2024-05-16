
DECLARE @ValoresAlmacen as TABLE
	(CodigoSapMaterial int, IdAlmacen int, NombreAlmacen varchar(50))

INSERT INTO @ValoresAlmacen
	(CodigoSapMaterial, IdAlmacen, NombreAlmacen)
VALUES
	(64194,		350,	'LINEA 3 R.Recupero'),
	(64194,		349,	'PUERTO'),
	(64194,		348,	'LOGISTICA'),
	(64194,		347,	'LINEA 3'),
	(64194,		346,	'LINEA 2'),
	(64194,		345,	'LINEA 1'),
	(64196,		254,	'No Productivo'),
	(64196,		350,	'LINEA 3 R.Recupero'),
	(64196,		351,	'CONTRATISTAS'),
	(64196,		352,	'PLAYA EXTERNA'),
	(172789,	344,	'Planta')

INSERT INTO Almacen (Id, Nombre)
SELECT distinct V.IdAlmacen, V.NombreAlmacen
FROM
	@ValoresAlmacen V left join
	Almacen A on V.IdAlmacen = A.Id
WHERE
	A.Id is null

INSERT INTO AlmacenMaterial (Almacen_Id, Material_Id)
SELECT distinct V.IdAlmacen, M.Id
FROM
	@ValoresAlmacen V inner join
	Material M on V.CodigoSapMaterial = M.CodigoSap left join
	AlmacenMaterial AM on V.IdAlmacen = AM.Almacen_Id and M.Id = AM.Material_Id
WHERE
	AM.Almacen_Id is null
