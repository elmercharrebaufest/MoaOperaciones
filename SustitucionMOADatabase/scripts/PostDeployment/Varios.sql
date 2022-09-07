IF NOT EXISTS (SELECT TOP 1 1 FROM Usuario WHERE Mail = 'moaoperaciones@molinosagro.com.ar') 
BEGIN
	insert into Usuario values ('moaoperaciones@molinosagro.com.ar','30715118773',1,2,null,'',0,null,null,'')
END

IF NOT EXISTS (SELECT TOP 1 1 FROM Configuracion WHERE Code = 'Soja200Desde') 
BEGIN
	insert into Configuracion values ('Soja200Desde','2022-09-05')
END
IF NOT EXISTS (SELECT TOP 1 1 FROM Configuracion WHERE Code = 'Soja200Hasta') 
BEGIN
	insert into Configuracion values ('Soja200Hasta','2022-09-30')
END
IF NOT EXISTS (SELECT TOP 1 1 FROM Configuracion WHERE Code = 'Soja200FechaCotizacion') 
BEGIN
	insert into Configuracion values ('Soja200FechaCotizacion','2015-09-01')
END
IF NOT EXISTS (SELECT TOP 1 1 FROM Configuracion WHERE Code = 'Soja200Cotizacion') 
BEGIN
	insert into Configuracion values ('Soja200Cotizacion','200')
END