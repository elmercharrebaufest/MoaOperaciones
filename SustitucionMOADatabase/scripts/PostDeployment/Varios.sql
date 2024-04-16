IF NOT EXISTS (SELECT TOP 1 1 FROM Usuario WHERE Mail = 'moaoperaciones@molinosagro.com.ar') 
BEGIN
	insert into Usuario values ('moaoperaciones@molinosagro.com.ar','30715118773',1,2,null,'',0,null,null,'',null,null)
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
IF NOT EXISTS (SELECT TOP 1 1 FROM Configuracion WHERE Code = 'EcheqLimiteCantidadAperturas') 
BEGIN
	insert into Configuracion values ('EcheqLimiteCantidadAperturas','4')
END
IF NOT EXISTS (SELECT TOP 1 1 FROM Configuracion WHERE Code = 'EcheqAforo') 
BEGIN
	insert into Configuracion values ('EcheqAforo','30')
END

IF EXISTS (SELECT *FROM OrdenDeCarga WHERE Estado = 13)
BEGIN
UPDATE  OrdenDeCarga SET EdicionRechazada = 1 WHERE  ID IN (SELECT ID FROM OrdenDeCarga WHERE estado = 13)
END 

IF NOT EXISTS (SELECT TOP 1 1 FROM Configuracion WHERE Code = 'DolarGirasolDesde') 
BEGIN
	insert into Configuracion values ('DolarGirasolDesde','2023-04-27')
END
IF NOT EXISTS (SELECT TOP 1 1 FROM Configuracion WHERE Code = 'DolarGirasolHasta') 
BEGIN
	insert into Configuracion values ('DolarGirasolHasta','2023-05-31')
END
IF NOT EXISTS (SELECT TOP 1 1 FROM Configuracion WHERE Code = 'DolarGirasolFechaCotizacion') 
BEGIN
	insert into Configuracion values ('DolarGirasolFechaCotizacion','2015-04-10')
END
IF NOT EXISTS (SELECT TOP 1 1 FROM Configuracion WHERE Code = 'DolarGirasolCotizacion') 
BEGIN
	insert into Configuracion values ('DolarGirasolCotizacion','300')
END

IF NOT EXISTS (SELECT TOP 1 1 FROM Configuracion WHERE Code = 'DolarMaizDesde') 
BEGIN
	insert into Configuracion values ('DolarMaizDesde','2023-07-25')
END
IF NOT EXISTS (SELECT TOP 1 1 FROM Configuracion WHERE Code = 'DolarMaizHasta') 
BEGIN
	insert into Configuracion values ('DolarMaizHasta','2023-08-31')
END
IF NOT EXISTS (SELECT TOP 1 1 FROM Configuracion WHERE Code = 'DolarMaizFechaCotizacion') 
BEGIN
	insert into Configuracion values ('DolarMaizFechaCotizacion','2015-07-25')
END
IF NOT EXISTS (SELECT TOP 1 1 FROM Configuracion WHERE Code = 'DolarMaizCotizacion') 
BEGIN
	insert into Configuracion values ('DolarMaizCotizacion','340')
END
IF NOT EXISTS (SELECT TOP 1 1 FROM Configuracion WHERE Code = 'PliegoDeGeneralidades') 
BEGIN
	insert into Configuracion values ('PliegoDeGeneralidades','https://b2cmoagro.blob.core.windows.net/moaopublic/PliegoGeneralidades.pdf')
END