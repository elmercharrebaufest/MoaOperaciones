
DECLARE @ValoresMaterial as TABLE
	(Nombre nvarchar(50), CodigoSap nvarchar(20), TablaSeccionMaterial int, ValidaSisaRuca bit, Abreviacion varchar(50), EsDerivadoGranario bit)

INSERT INTO @ValoresMaterial
VALUES
	-- Nombre										CodigoSap	Seccion		ValidaSisaRuca	Abreviacion			EsDerivadoGranario
	('94705 - ACEITE DE SOJA CRUDO A GRANEL',		'94705',	1,			1,				'Ac. Soja',			0), 
	('98855 - ACEITE DE SOJA NEUTRALIZADO',			'98855',	1,			0,				'Ac. Neutro',		0),
	('94687 - ACEITE GIRASOL CRUDO',				'94687',	1,			1,				'Ac. Girasol',		0),
	('99098 - ACEITE METILADO DE SOJA',				'99098',	1,			0,				'Ac. Metilado',		0),
	('50866 - HARINA DE SOJA HIPRO',				'50866',	1,			0,				'Harina hipro',		0), 
	('99059 - LECITINA DE GIRASOL',					'99059',	1,			0,				'Lecit. Girasol',	0),
	('99056 - LECITINA DE SOJA',					'99056',	1,			0,				'Lecit. Soja',		0),
	('99704 - PELLET DE CASCARA DE SOJA A GRANEL',	'99704',	1,			1,				'P. Cáscara',		0),
	('99710 - PELLET DE GIRASOL',					'99710',	1,			0,				'P. Girasol',		0),
	('99709 - PELLET DE GIRASOL INTEGRAL',			'99709',	1,			1,				'P. Girasol Int',	0),

	('94705 - ACEITE DE SOJA CRUDO A GRANEL',		'94705',	2,			1,				'Ac. Soja',			1), 
	('94687 - ACEITE GIRASOL CRUDO',				'94687',	2,			1,				'Ac. Girasol',		1),
	('50866 - HARINA DE SOJA HIPRO',				'50866',	2,			0,				'Harina hipro',		0), 
	('99704 - PELLET DE CASCARA DE SOJA A GRANEL',	'99704',	2,			1,				'P. Cáscara',		1), 
	('99709 - PELLET DE GIRASOL INTEGRAL',			'99709',	2,			1,				'P. Girasol Int',	1),

	( '64195 - CHATARRA',							'64195',	3,			0,				'Chatarra',			0),
	( '64207 - GOMA EXTRACCION',					'64207',	3,			0,				'Goma extracc.',	0),
	( '63699 - MATERIAL RECICLABLE - NO PELIGROSO',	'63699',	3,			0,				'Mat. rec. no pel.',0),
	( '64200 - RESIDUOS LIQUIDOS URBANOS',			'64200',	3,			0,				'Resid. líq. urb.',	0),
	( '64194 - RESIDUOS ORGANICOS',					'64194',	3,			1,				'Residuos org.',	0),
	( '64199 - RESIDUOS PELIGROSOS',				'64199',	3,			0,				'Resid. peligr.',	0),
	( '64196 - RESIDUOS SOLIDOS URBANOS',			'64196',	3,			0,				'Resid. sól. urb',	0),
	('172789 - SUELO SELECCIONADO',					'172789',	3,			0,				'Suelo selecc.',	0),
	('99131 - BIODIESEL (METILESTER DE SOJA) A GRANEL',	'99131',2,			0,				'BIO',				0)

-- Actualización de materiales existentes
UPDATE Material
SET
	Nombre = V.Nombre,
	ValidaSisaRuca = V.ValidaSisaRuca,
	Abreviacion = V.Abreviacion,
	EsDerivadoGranario = V.EsDerivadoGranario
FROM
	@ValoresMaterial V left join
	Material M on
		M.TablaSeccionMaterial = V.TablaSeccionMaterial and
		M.CodigoSap = V.CodigoSap
WHERE
	M.Id is not null

-- Alta de materiales nuevos
INSERT INTO Material
	(Nombre, CodigoSap, TablaSeccionMaterial, ValidaSisaRuca, Abreviacion, EsDerivadoGranario)
SELECT
	V.Nombre, V.CodigoSap, V.TablaSeccionMaterial, V.ValidaSisaRuca, V.Abreviacion, V.EsDerivadoGranario
FROM
	@ValoresMaterial V left join
	Material M on
		M.TablaSeccionMaterial = V.TablaSeccionMaterial and
		M.CodigoSap = V.CodigoSap
WHERE
	M.Id is null