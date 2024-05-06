-- Materiales con TablaSeccionMaterial = 1
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.Material WHERE CodigoSap = '99704' and Nombre = '99704 - PELLET DE CASCARA DE SOJA A GRANEL' ) 
BEGIN 
	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial) VALUES('99704 - PELLET DE CASCARA DE SOJA A GRANEL','99704','1') 
END

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.Material WHERE CodigoSap = '94705' and Nombre = '94705 - ACEITE DE SOJA CRUDO A GRANEL' ) 
BEGIN 
	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial) VALUES('94705 - ACEITE DE SOJA CRUDO A GRANEL','94705','1') 
END

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.Material WHERE CodigoSap = '50866' and Nombre = '50866 - HARINA DE SOJA HIPRO' ) 
BEGIN 
	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial) VALUES('50866 - HARINA DE SOJA HIPRO','50866','1') 
END

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.Material WHERE CodigoSap = '94687' and Nombre = '94687 - ACEITE GIRASOL CRUDO' ) 
BEGIN 
	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial) VALUES('94687 - ACEITE GIRASOL CRUDO','94687','1')
END

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.Material WHERE CodigoSap = '99059' and Nombre = '99059 - LECITINA DE GIRASOL' ) 
BEGIN 
	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial) VALUES('99059 - LECITINA DE GIRASOL','99059','1')
END

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.Material WHERE CodigoSap = '99056' and Nombre = '99056 - LECITINA DE SOJA' ) 
BEGIN 
	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial) VALUES('99056 - LECITINA DE SOJA','99056','1')
END

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.Material WHERE CodigoSap = '98855' and Nombre = '98855 - ACEITE DE SOJA NEUTRALIZADO' ) 
BEGIN 
	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial) VALUES('98855 - ACEITE DE SOJA NEUTRALIZADO','98855','1')
END

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.Material WHERE CodigoSap = '99098' and Nombre = '99098 - ACEITE METILADO DE SOJA' ) 
BEGIN 
	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial) VALUES('99098 - ACEITE METILADO DE SOJA','99098','1')
END

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.Material WHERE CodigoSap = '99710' and Nombre = '99710 - PELLET DE GIRASOL' ) 
BEGIN 
	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial) VALUES('99710 - PELLET DE GIRASOL','99710','1')
END

-- Materiales con TablaSeccionMaterial = 2

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.Material WHERE CodigoSap = '99704' and Nombre = '99704 - PELLET DE CASCARA DE SOJA A GRANEL' and TablaSeccionMaterial = '2') 
BEGIN 
	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial) VALUES('99704 - PELLET DE CASCARA DE SOJA A GRANEL','99704','2') 
END

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.Material WHERE CodigoSap = '94705' and Nombre = '94705 - ACEITE DE SOJA CRUDO A GRANEL' and TablaSeccionMaterial = '2') 
BEGIN 
	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial) VALUES('94705 - ACEITE DE SOJA CRUDO A GRANEL','94705','2') 
END

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.Material WHERE CodigoSap = '50866' and Nombre = '50866 - HARINA DE SOJA HIPRO' and TablaSeccionMaterial = '2') 
BEGIN 
	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial) VALUES('50866 - HARINA DE SOJA HIPRO','50866','2') 
END

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.Material WHERE CodigoSap = '94687' and Nombre = '94687 - ACEITE GIRASOL CRUDO' and TablaSeccionMaterial = '2') 
BEGIN 
	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial) VALUES('94687 - ACEITE GIRASOL CRUDO','94687','2')
END

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.Material WHERE CodigoSap = '99709' and Nombre = '99709 - PELLET DE GIRASOL INTEGRAL' and TablaSeccionMaterial = '1') 
BEGIN 
	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial) VALUES('99709 - PELLET DE GIRASOL INTEGRAL','99709','1')
END

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.Material WHERE CodigoSap = '99709' and Nombre = '99709 - PELLET DE GIRASOL INTEGRAL' and TablaSeccionMaterial = '2') 
BEGIN 
	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial) VALUES('99709 - PELLET DE GIRASOL INTEGRAL','99709','2')
END

-- Materiales con TablaSeccionMaterial = 3

IF NOT EXISTS (SELECT top 1 1 FROM dbo.Material WHERE CodigoSap = '172789' and TablaSeccionMaterial = 3)
BEGIN
	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial, ValidaSisaRuca, Abreviacion)
	VALUES ('172789 - SUELO SELECCIONADO', '172789', 3, 0, 'Suelo selecc.')
END

IF NOT EXISTS (SELECT top 1 1 FROM dbo.Material WHERE CodigoSap = '64194' and TablaSeccionMaterial = 3)
BEGIN
	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial, ValidaSisaRuca, Abreviacion)
	VALUES ('64194 - RESIDUOS ORGANICOS', '64194', 3, 1, 'Residuos org.')
END

IF NOT EXISTS (SELECT top 1 1 FROM dbo.Material WHERE CodigoSap = '64196' and TablaSeccionMaterial = 3)
BEGIN
	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial, ValidaSisaRuca, Abreviacion)
	VALUES ('64196 - RESIDUOS SOLIDOS URBANOS', '64196', 3, 0, 'Resid. sól. urb')
END

-- Updates

UPDATE Material SET Nombre = '99709 - PELLET DE GIRASOL INTEGRAL' WHERE CodigoSap = '99709'

UPDATE Material SET ValidaSisaRuca = 1 WHERE
	CodigoSap = 94687 or	-- ACEITE GIRASOL CRUDO
	CodigoSap = 99709 or	-- PELLET DE GIRASOL INTEGRAL
	CodigoSap = 94705 or	-- ACEITE DE SOJA CRUDO A GRANEL
	CodigoSap = 99704;		-- PELLET DE CASCARA DE SOJA A GRANEL

UPDATE Material SET Abreviacion = 'Harina hipro' WHERE CodigoSap = 50866;
UPDATE Material SET Abreviacion = 'Ac. Girasol' WHERE CodigoSap = 94687;
UPDATE Material SET Abreviacion = 'Ac. Soja' WHERE CodigoSap = 94705;
UPDATE Material SET Abreviacion = 'Ac. Neutro' WHERE CodigoSap = 98855;
UPDATE Material SET Abreviacion = 'Lecit. Soja' WHERE CodigoSap = 99056;
UPDATE Material SET Abreviacion = 'Lecit. Girasol' WHERE CodigoSap = 99059;
UPDATE Material SET Abreviacion = 'Ac. Metilado' WHERE CodigoSap = 99098;
UPDATE Material SET Abreviacion = 'P. Cáscara' WHERE CodigoSap = 99704;
UPDATE Material SET Abreviacion = 'P. Girasol Int' WHERE CodigoSap = 99709;
UPDATE Material SET Abreviacion = 'P. Girasol' WHERE CodigoSap = 99710;
