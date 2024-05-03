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
	(Tabla = 'Centro' AND CodigoSap IN ('1029', '1075', '1071', '1072', '1030', '1035', '1036', '1126', '1127', '1001'))
	OR
	(Tabla = 'GrupoCompras' AND CodigoSap IN ('300', '600', '103', '215', '225', '228', '230', '231', '241', '401', '430', '455', '462', '601', '602', '603', '604', '605', '606', '607', '608', '609', '611', '612', '613', '614', '615', '616', '617', '618', '619', '620', '621', '622', '623', '624', '626', '627', '628', '629', '630', '631', '810'));

-- MonedaConversion

-- Continuación del script
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.MonedaConversion WHERE MonedaCodigo = 'ROL') 
    INSERT INTO MonedaConversion(MonedaCodigo, CantidadDecimal) VALUES ('ROL', 0)

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.MonedaConversion WHERE MonedaCodigo = 'RWF') 
    INSERT INTO MonedaConversion(MonedaCodigo, CantidadDecimal) VALUES ('RWF', 0)

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.MonedaConversion WHERE MonedaCodigo = 'TJR') 
    INSERT INTO MonedaConversion(MonedaCodigo, CantidadDecimal) VALUES ('TJR', 0)

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.MonedaConversion WHERE MonedaCodigo = 'TMM') 
    INSERT INTO MonedaConversion(MonedaCodigo, CantidadDecimal) VALUES ('TMM', 0)

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.MonedaConversion WHERE MonedaCodigo = 'TND') 
    INSERT INTO MonedaConversion(MonedaCodigo, CantidadDecimal) VALUES ('TND', 3)

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.MonedaConversion WHERE MonedaCodigo = 'TPE') 
    INSERT INTO MonedaConversion(MonedaCodigo, CantidadDecimal) VALUES ('TPE', 0)

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.MonedaConversion WHERE MonedaCodigo = 'TRL') 
    INSERT INTO MonedaConversion(MonedaCodigo, CantidadDecimal) VALUES ('TRL', 0)

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.MonedaConversion WHERE MonedaCodigo = 'UGX') 
    INSERT INTO MonedaConversion(MonedaCodigo, CantidadDecimal) VALUES ('UGX', 0)

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.MonedaConversion WHERE MonedaCodigo = 'USDM') 
    INSERT INTO MonedaConversion(MonedaCodigo, CantidadDecimal) VALUES ('USDM', 3)

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.MonedaConversion WHERE MonedaCodigo = 'USDN') 
    INSERT INTO MonedaConversion(MonedaCodigo, CantidadDecimal) VALUES ('USDN', 5)

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.MonedaConversion WHERE MonedaCodigo = 'USDP') 
    INSERT INTO MonedaConversion(MonedaCodigo, CantidadDecimal) VALUES ('USDP', 5)

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.MonedaConversion WHERE MonedaCodigo = 'VND') 
    INSERT INTO MonedaConversion(MonedaCodigo, CantidadDecimal) VALUES ('VND', 0)

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.MonedaConversion WHERE MonedaCodigo = 'VUV') 
    INSERT INTO MonedaConversion(MonedaCodigo, CantidadDecimal) VALUES ('VUV', 0)

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.MonedaConversion WHERE MonedaCodigo = 'XAF') 
    INSERT INTO MonedaConversion(MonedaCodigo, CantidadDecimal) VALUES ('XAF', 0)

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.MonedaConversion WHERE MonedaCodigo = 'XOF') 
    INSERT INTO MonedaConversion(MonedaCodigo, CantidadDecimal) VALUES ('XOF', 0)

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.MonedaConversion WHERE MonedaCodigo = 'XPF') 
    INSERT INTO MonedaConversion(MonedaCodigo, CantidadDecimal) VALUES ('XPF', 0)

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.MonedaConversion WHERE MonedaCodigo = 'ZRN') 
    INSERT INTO MonedaConversion(MonedaCodigo, CantidadDecimal) VALUES ('ZRN', 0)


--Incoterms

IF NOT EXISTS(SELECT 1 FROM tablasap WHERE Tabla = 'INCOTERMS' AND Codigo = 'CPT') BEGIN insert into tablasap (Tabla,Codigo,CodigoSap,Descripcion)values ('INCOTERMS','CPT','CPT','Costo y Flete pagado hasta terminal')END
IF NOT EXISTS(SELECT 1 FROM tablasap WHERE Tabla = 'INCOTERMS' AND Codigo = 'CIF') BEGIN insert into tablasap (Tabla,Codigo,CodigoSap,Descripcion)values ('INCOTERMS','CIF','CIF','Costo, seguro y flete')END
IF NOT EXISTS(SELECT 1 FROM tablasap WHERE Tabla = 'INCOTERMS' AND Codigo = 'CFR') BEGIN insert into tablasap (Tabla,Codigo,CodigoSap,Descripcion)values ('INCOTERMS','CFR','CFR','Costo y Flete')END
IF NOT EXISTS(SELECT 1 FROM tablasap WHERE Tabla = 'INCOTERMS' AND Codigo = 'CIP') BEGIN insert into tablasap (Tabla,Codigo,CodigoSap,Descripcion)values ('INCOTERMS','CIP','CIP','Porte y seguro pagado hasta')END
IF NOT EXISTS(SELECT 1 FROM tablasap WHERE Tabla = 'INCOTERMS' AND Codigo = 'FAS') BEGIN insert into tablasap (Tabla,Codigo,CodigoSap,Descripcion)values ('INCOTERMS','FAS','FAS','Libre al costado del buque')END
IF NOT EXISTS(SELECT 1 FROM tablasap WHERE Tabla = 'INCOTERMS' AND Codigo = 'FAC') BEGIN insert into tablasap (Tabla,Codigo,CodigoSap,Descripcion)values ('INCOTERMS','FCA','FCA','Libre transportista')END
IF NOT EXISTS(SELECT 1 FROM tablasap WHERE Tabla = 'INCOTERMS' AND Codigo = 'FOB') BEGIN insert into tablasap (Tabla,Codigo,CodigoSap,Descripcion)values ('INCOTERMS','FOB','FOB','Libre a bordo')END
IF NOT EXISTS(SELECT 1 FROM tablasap WHERE Tabla = 'INCOTERMS' AND Codigo = 'EXW') BEGIN insert into tablasap (Tabla,Codigo,CodigoSap,Descripcion)values ('INCOTERMS','EXW','EXW','Entrega en fabrica  vendedor')END
IF NOT EXISTS(SELECT 1 FROM tablasap WHERE Tabla = 'INCOTERMS' AND Codigo = 'DAF') BEGIN insert into tablasap (Tabla,Codigo,CodigoSap,Descripcion)values ('INCOTERMS','DAF','DAF','Entrega en frontera')END
IF NOT EXISTS(SELECT 1 FROM tablasap WHERE Tabla = 'INCOTERMS' AND Codigo = 'DAP') BEGIN insert into tablasap (Tabla,Codigo,CodigoSap,Descripcion)values ('INCOTERMS','DAP','DAP','Entrega en plaza')END
IF NOT EXISTS(SELECT 1 FROM tablasap WHERE Tabla = 'INCOTERMS' AND Codigo = 'DAT') BEGIN insert into tablasap (Tabla,Codigo,CodigoSap,Descripcion)values ('INCOTERMS','DAT','DAT','Entrega en terminal')END
IF NOT EXISTS(SELECT 1 FROM tablasap WHERE Tabla = 'INCOTERMS' AND Codigo = 'DDP') BEGIN insert into tablasap (Tabla,Codigo,CodigoSap,Descripcion)values ('INCOTERMS','DDP','DDP','Ent. destino, derecho  pagados')END

-- Condiciones de pago

IF NOT EXISTS(SELECT 1 FROM tablasap WHERE Tabla = 'CondicionesDePago' AND Codigo = '00PP') BEGIN insert into tablasap (Tabla,Codigo,CodigoSap,Descripcion)values ('CondicionesDePago','00PP','00PP','Previo Pago') END
IF NOT EXISTS(SELECT 1 FROM tablasap WHERE Tabla = 'CondicionesDePago' AND Codigo = '0011') BEGIN insert into tablasap (Tabla,Codigo,CodigoSap,Descripcion)values ('CondicionesDePago','0011','0011','Pagadero inmediatamente sin DPP') END
IF NOT EXISTS(SELECT 1 FROM tablasap WHERE Tabla = 'CondicionesDePago' AND Codigo = '0007') BEGIN insert into tablasap (Tabla,Codigo,CodigoSap,Descripcion)values ('CondicionesDePago','0007','0007','Pagadero dentro de los 7 días sin DPP') END
IF NOT EXISTS(SELECT 1 FROM tablasap WHERE Tabla = 'CondicionesDePago' AND Codigo = '0015') BEGIN insert into tablasap (Tabla,Codigo,CodigoSap,Descripcion)values ('CondicionesDePago','0015','0015','Pagadero dentro de los 15 días sin DPP') END
IF NOT EXISTS(SELECT 1 FROM tablasap WHERE Tabla = 'CondicionesDePago' AND Codigo = '0021') BEGIN insert into tablasap (Tabla,Codigo,CodigoSap,Descripcion)values ('CondicionesDePago','0021','0021','Pagadero dentro de los 21 días sin DPP') END
IF NOT EXISTS(SELECT 1 FROM tablasap WHERE Tabla = 'CondicionesDePago' AND Codigo = '0030') BEGIN insert into tablasap (Tabla,Codigo,CodigoSap,Descripcion)values ('CondicionesDePago','0030','0030','Pagadero dentro de los 30 días sin DPP') END
IF NOT EXISTS(SELECT 1 FROM tablasap WHERE Tabla = 'CondicionesDePago' AND Codigo = '0045') BEGIN insert into tablasap (Tabla,Codigo,CodigoSap,Descripcion)values ('CondicionesDePago','0045','0045','Pagadero dentro de los 45 días sin DPP') END
IF NOT EXISTS(SELECT 1 FROM tablasap WHERE Tabla = 'CondicionesDePago' AND Codigo = '0060') BEGIN insert into tablasap (Tabla,Codigo,CodigoSap,Descripcion)values ('CondicionesDePago','0060','0060','Pagadero dentro de los 60 días sin DPP') END
IF NOT EXISTS(SELECT 1 FROM tablasap WHERE Tabla = 'CondicionesDePago' AND Codigo = '0075') BEGIN insert into tablasap (Tabla,Codigo,CodigoSap,Descripcion)values ('CondicionesDePago','0075','0075','Pagadero dentro de los 75 días sin DPP') END
IF NOT EXISTS(SELECT 1 FROM tablasap WHERE Tabla = 'CondicionesDePago' AND Codigo = '0090') BEGIN insert into tablasap (Tabla,Codigo,CodigoSap,Descripcion)values ('CondicionesDePago','0090','0090','Pagadero dentro de los 90 días sin DPP') END
IF NOT EXISTS (SELECT TOP 1 1 FROM Configuracion WHERE Code = 'CosechaParaEnvioUcropit') 
BEGIN
	insert into Configuracion values ('CosechaParaEnvioUcropit','22-23')
END

IF NOT EXISTS (SELECT TOP 1 1 FROM Configuracion WHERE Code = 'CosechaParaEnvioUcropitTope') 
BEGIN
	insert into Configuracion values ('CosechaParaEnvioUcropitTope','200')
END


IF NOT EXISTS (SELECT TOP 1 1 FROM Configuracion WHERE Code = 'HoraCortePesificaciones') 
BEGIN
	insert into Configuracion values ('HoraCortePesificaciones','13:00')
END

IF NOT EXISTS (SELECT TOP 1 1 FROM Configuracion WHERE Code = 'DiasParaDiscreparCalidadesDescarga') 
BEGIN
	insert into Configuracion values ('DiasParaDiscreparCalidadesDescarga','-7')
END

IF NOT EXISTS (SELECT TOP 1 1 FROM Configuracion WHERE Code = 'FechaLimiteCamposSustentables') 
BEGIN
	insert into Configuracion values ('FechaLimiteCamposSustentables','2024-02-22 00:00')
END
