IF NOT EXISTS (SELECT TOP 1 1 FROM Usuario WHERE Mail = 'moaoperaciones@molinosagro.com.ar') 
BEGIN
	insert into Usuario values ('moaoperaciones@molinosagro.com.ar','30715118773',1,2,null,'',0,null,null,'',null)
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

update CentroDireccion set RegionSap_Id = '9' where CodigoSap = '1001';
update CentroDireccion set RegionSap_Id = '20' where CodigoSap = '1029';
update CentroDireccion set RegionSap_Id = '17' where CodigoSap = '1075';
update CentroDireccion set RegionSap_Id = '23' where CodigoSap = '1072';
update CentroDireccion set RegionSap_Id = '11' where CodigoSap = '1030';
update CentroDireccion set RegionSap_Id = '9' where CodigoSap = '1035';
update CentroDireccion set RegionSap_Id = '9' where CodigoSap = '1036';
update CentroDireccion set RegionSap_Id = '11' where CodigoSap = '1126';
update CentroDireccion set RegionSap_Id = '21' where CodigoSap = '1127';

update CentroDireccion set RegionSap_Id = '20' where CodigoSap = '1034' 
												  or CodigoSap = '1067' 
												  or CodigoSap = '1068'
												  or CodigoSap = '1069'
												  or CodigoSap = '1070'
												  or CodigoSap = '1071'
												  or CodigoSap = '1073'
												  or CodigoSap = '1074'
												  or CodigoSap = '1086'
												  or CodigoSap = '1087'
												  or CodigoSap = '1164'
												  or CodigoSap = '1165'
												  or CodigoSap = '1166'
												  or CodigoSap = '1167'
												  or CodigoSap = '1168'
												  or CodigoSap = '1169'
												  or CodigoSap = '1170'
												  or CodigoSap = '1500'
												  or CodigoSap = '1600'
												  or CodigoSap = '2000'
												  or CodigoSap = '2001'
												  or CodigoSap = '5000'
												  or CodigoSap = '6000'
												  or CodigoSap = '8107'
												  or CodigoSap = '8118'
												  or CodigoSap = '9000'; 

--Habilitar centros y grupos de compra usados por los compradores en TablaSap
UPDATE TablaSap SET FiltroComprador = 1
WHERE
	(Tabla = 'Centro' AND CodigoSap IN ('1029', '1075', '1072', '1030', '1035', '1036', '1126', '1127', '1001'))
	OR
	(Tabla = 'GrupoCompras' AND CodigoSap IN ('300', '600', '103', '215', '225', '228', '230', '231', '241', '401', '430', '455', '462', '601', '602', '603', '604', '605', '606', '607', '608', '609', '611', '612', '613', '614', '615', '616', '617', '618', '619', '620', '621', '622', '623', '624', '626', '627', '628', '629', '630', '631', '810'));

