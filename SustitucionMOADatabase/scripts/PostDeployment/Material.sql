
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
	('172789 - SUELO SELECCIONADO',					'172789',	3,			0,				'Suelo selecc.',	0)

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


-- Materiales con TablaSeccionMaterial = 1
--IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.Material WHERE CodigoSap = '99704' and Nombre = '99704 - PELLET DE CASCARA DE SOJA A GRANEL' ) 
--BEGIN 
--	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial) VALUES('99704 - PELLET DE CASCARA DE SOJA A GRANEL','99704','1') 
--END

--IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.Material WHERE CodigoSap = '94705' and Nombre = '94705 - ACEITE DE SOJA CRUDO A GRANEL' ) 
--BEGIN 
--	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial) VALUES('94705 - ACEITE DE SOJA CRUDO A GRANEL','94705','1') 
--END

--IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.Material WHERE CodigoSap = '50866' and Nombre = '50866 - HARINA DE SOJA HIPRO' ) 
--BEGIN 
--	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial) VALUES('50866 - HARINA DE SOJA HIPRO','50866','1') 
--END

--IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.Material WHERE CodigoSap = '94687' and Nombre = '94687 - ACEITE GIRASOL CRUDO' ) 
--BEGIN 
--	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial) VALUES('94687 - ACEITE GIRASOL CRUDO','94687','1')
--END

--IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.Material WHERE CodigoSap = '99059' and Nombre = '99059 - LECITINA DE GIRASOL' ) 
--BEGIN 
--	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial) VALUES('99059 - LECITINA DE GIRASOL','99059','1')
--END

--IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.Material WHERE CodigoSap = '99056' and Nombre = '99056 - LECITINA DE SOJA' ) 
--BEGIN 
--	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial) VALUES('99056 - LECITINA DE SOJA','99056','1')
--END

--IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.Material WHERE CodigoSap = '98855' and Nombre = '98855 - ACEITE DE SOJA NEUTRALIZADO' ) 
--BEGIN 
--	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial) VALUES('98855 - ACEITE DE SOJA NEUTRALIZADO','98855','1')
--END

--IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.Material WHERE CodigoSap = '99098' and Nombre = '99098 - ACEITE METILADO DE SOJA' ) 
--BEGIN 
--	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial) VALUES('99098 - ACEITE METILADO DE SOJA','99098','1')
--END

--IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.Material WHERE CodigoSap = '99710' and Nombre = '99710 - PELLET DE GIRASOL' ) 
--BEGIN 
--	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial) VALUES('99710 - PELLET DE GIRASOL','99710','1')
--END

---- Materiales con TablaSeccionMaterial = 2

--IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.Material WHERE CodigoSap = '99704' and Nombre = '99704 - PELLET DE CASCARA DE SOJA A GRANEL' and TablaSeccionMaterial = '2') 
--BEGIN 
--	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial) VALUES('99704 - PELLET DE CASCARA DE SOJA A GRANEL','99704','2') 
--END

--IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.Material WHERE CodigoSap = '94705' and Nombre = '94705 - ACEITE DE SOJA CRUDO A GRANEL' and TablaSeccionMaterial = '2') 
--BEGIN 
--	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial) VALUES('94705 - ACEITE DE SOJA CRUDO A GRANEL','94705','2') 
--END

--IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.Material WHERE CodigoSap = '50866' and Nombre = '50866 - HARINA DE SOJA HIPRO' and TablaSeccionMaterial = '2') 
--BEGIN 
--	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial) VALUES('50866 - HARINA DE SOJA HIPRO','50866','2') 
--END

--IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.Material WHERE CodigoSap = '94687' and Nombre = '94687 - ACEITE GIRASOL CRUDO' and TablaSeccionMaterial = '2') 
--BEGIN 
--	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial) VALUES('94687 - ACEITE GIRASOL CRUDO','94687','2')
--END

--IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.Material WHERE CodigoSap = '99709' and Nombre = '99709 - PELLET DE GIRASOL INTEGRAL' and TablaSeccionMaterial = '1') 
--BEGIN 
--	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial) VALUES('99709 - PELLET DE GIRASOL INTEGRAL','99709','1')
--END

--IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.Material WHERE CodigoSap = '99709' and Nombre = '99709 - PELLET DE GIRASOL INTEGRAL' and TablaSeccionMaterial = '2') 
--BEGIN 
--	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial) VALUES('99709 - PELLET DE GIRASOL INTEGRAL','99709','2')
--END

---- Materiales con TablaSeccionMaterial = 3

--IF NOT EXISTS (SELECT top 1 1 FROM dbo.Material WHERE CodigoSap = '172789' and TablaSeccionMaterial = 3)
--BEGIN
--	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial, ValidaSisaRuca, Abreviacion)
--	VALUES ('172789 - SUELO SELECCIONADO', '172789', 3, 0, 'Suelo selecc.')
--END

--IF NOT EXISTS (SELECT top 1 1 FROM dbo.Material WHERE CodigoSap = '64194' and TablaSeccionMaterial = 3)
--BEGIN
--	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial, ValidaSisaRuca, Abreviacion)
--	VALUES ('64194 - RESIDUOS ORGANICOS', '64194', 3, 1, 'Residuos org.')
--END

--IF NOT EXISTS (SELECT top 1 1 FROM dbo.Material WHERE CodigoSap = '64196' and TablaSeccionMaterial = 3)
--BEGIN
--	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial, ValidaSisaRuca, Abreviacion)
--	VALUES ('64196 - RESIDUOS SOLIDOS URBANOS', '64196', 3, 0, 'Resid. sól. urb')
--END

--IF NOT EXISTS (SELECT top 1 1 FROM dbo.Material WHERE CodigoSap = '64200' and TablaSeccionMaterial = 3)
--BEGIN
--	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial, ValidaSisaRuca, Abreviacion)
--	VALUES ('64200 - RESIDUOS LIQUIDOS URBANOS', '64200', 3, 0, 'Resid. líq. urb.')
--END

--IF NOT EXISTS (SELECT top 1 1 FROM dbo.Material WHERE CodigoSap = '64199' and TablaSeccionMaterial = 3)
--BEGIN
--	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial, ValidaSisaRuca, Abreviacion)
--	VALUES ('64199 - RESIDUOS PELIGROSOS', '64199', 3, 0, 'Resid. peligr.')
--END

--IF NOT EXISTS (SELECT top 1 1 FROM dbo.Material WHERE CodigoSap = '64207' and TablaSeccionMaterial = 3)
--BEGIN
--	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial, ValidaSisaRuca, Abreviacion)
--	VALUES ('64207 - GOMA EXTRACCION', '64207', 3, 0, 'Goma extracc.')
--END

--IF NOT EXISTS (SELECT top 1 1 FROM dbo.Material WHERE CodigoSap = '64195' and TablaSeccionMaterial = 3)
--BEGIN
--	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial, ValidaSisaRuca, Abreviacion)
--	VALUES ('64195 - CHATARRA', '64195', 3, 0, 'Chatarra')
--END

--IF NOT EXISTS (SELECT top 1 1 FROM dbo.Material WHERE CodigoSap = '63699' and TablaSeccionMaterial = 3)
--BEGIN
--	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial, ValidaSisaRuca, Abreviacion)
--	VALUES ('63699 - MATERIAL RECICLABLE - NO PELIGROSO', '63699', 3, 0, 'Mat. rec. no pel.')
--END

-- Updates

--UPDATE Material SET Nombre = '99709 - PELLET DE GIRASOL INTEGRAL' WHERE CodigoSap = '99709'

--UPDATE Material SET ValidaSisaRuca = 1 WHERE
--	CodigoSap = 94687 or	-- ACEITE GIRASOL CRUDO
--	CodigoSap = 99709 or	-- PELLET DE GIRASOL INTEGRAL
--	CodigoSap = 94705 or	-- ACEITE DE SOJA CRUDO A GRANEL
--	CodigoSap = 99704;		-- PELLET DE CASCARA DE SOJA A GRANEL

--UPDATE Material SET Abreviacion = 'Harina hipro' WHERE CodigoSap = 50866;
--UPDATE Material SET Abreviacion = 'Ac. Girasol' WHERE CodigoSap = 94687;
--UPDATE Material SET Abreviacion = 'Ac. Soja' WHERE CodigoSap = 94705;
--UPDATE Material SET Abreviacion = 'Ac. Neutro' WHERE CodigoSap = 98855;
--UPDATE Material SET Abreviacion = 'Lecit. Soja' WHERE CodigoSap = 99056;
--UPDATE Material SET Abreviacion = 'Lecit. Girasol' WHERE CodigoSap = 99059;
--UPDATE Material SET Abreviacion = 'Ac. Metilado' WHERE CodigoSap = 99098;
--UPDATE Material SET Abreviacion = 'P. Cáscara' WHERE CodigoSap = 99704;
--UPDATE Material SET Abreviacion = 'P. Girasol Int' WHERE CodigoSap = 99709;
--UPDATE Material SET Abreviacion = 'P. Girasol' WHERE CodigoSap = 99710;
