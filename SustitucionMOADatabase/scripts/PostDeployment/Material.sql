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

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.Material WHERE CodigoSap = '99709' and Nombre = '99709 - PELLET DE GIRASOL INTEGRAL' ) 
BEGIN 
	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial) VALUES('99709 - PELLET DE GIRASOL INTEGRAL','99709','1')
END

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

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.Material WHERE CodigoSap = '99709' and Nombre = '99709 - PELLET DE GIRASOL' and TablaSeccionMaterial = '2') 
BEGIN 
	INSERT INTO Material(Nombre, CodigoSap, TablaSeccionMaterial) VALUES('99709 - PELLET DE GIRASOL','99709','2')
END
