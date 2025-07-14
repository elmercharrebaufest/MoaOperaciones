--Rol
--select *,'IF NOT EXISTS(SELECT 1 FROM Rol WHERE Codigo = '''+Codigo+''' AND Nombre = '''+Nombre+''') BEGIN INSERT INTO Rol values ('''+Codigo+''','''+Nombre+''','+ltrim(EsEditable)+')'from rol

-- UPDATES
UPDATE Rol SET Codigo = 'PROVG' WHERE Codigo = 'PROVGC'
UPDATE Rol SET Nombre = 'CERTIFICACIÓN DE SERVICIOS EXT' WHERE Codigo = 'CERTIFICACION EXTERNA'

-- TABLA DE ROLES
DECLARE @ValoresRol as TABLE
    (Codigo varchar(max), Nombre varchar(max), EsEditable bit, TipoRol int)

INSERT INTO @ValoresRol
VALUES
    ('ACCESO QR',		    'Acceso QR',		    1, 2),
    ('ACT',		            'ACTUALIZACIÓN',	    1, 3),
    ('ADM',		            'ADMINISTRACION',	    1, 1),
    ('ADMCM05',		        'Gestion CM05',		    1, 2),
    ('ADMIN_CURSOS',        'CURSOS ADMIN',         1, 1),
    ('ADMINCCSS',		    'ADMIN CAMPO SUSTENTABLE', 1, 1),
    ('ADMINCONTMA',		    'ADMIN CONTABILIZACION MES ANTERIOR', 1, 1),
    ('ADMINPLATCOMPRAS',    'ADMIN PLATAFORMA COMPRAS', 1, 1),
    ('ADU',		            'ADUANA',		        1, 2),
    ('AIGRAN',		        'ALTA INTERNA GRANOS',	1, 1),
    ('AINOGRAN',	        'ALTA INTERNA NO GRANOS', 1, 1),
    ('ALLES',		        'VER TODOS LOS ESTADOS DE ES', 1, 1),
    ('ALUMNO_CURSOS',       'ALUMNO CURSOS',        1, 2),
    ('ANUL',		        'ANULADOR',		        1, 1),
    ('API_ORDCARGA',        'API ORDENES DE CARGA', 0, 2),
    ('API_ORDRESIDUOS',     'API ORDENES RESIDUOS', 0, 1),
    ('APIKEY',		        'APIKEY',		        1, 1),
    ('APLCCPP ADMIN',		'APLICACION CCPP ADMIN', 1, 1),
    ('APLCCPP',		        'APLICACION CCPP',	    1, 2),
    ('APP',		            'APLICACIONES',	        1, 3),
    ('APRO',		        'APROBADOR',		    1, 1),
    ('AUDITOR COMPRAS',		'AUDITOR COMPRAS',	    1, 1),
    ('BOL',		            'BOLETOS',		        1, 3),
    ('CAL',		            'CALIDADES',	        1, 3),
    ('CERTIFICACION',       'CERTIFICACIÓN DE SERVICIOS', 1, 1),
    ('CERTIFICACION EXTERNA','CERTIFICACIÓN DE SERVICIOS EXT', 1, 2),
    ('CLIENT',	            'SOLO CLIENTES',	    1, 2),
    ('COM',		            'COMISIONES',	        1, 3),
    ('COMERCIAL',		    'COMERCIAL',	        1, 1),
    ('COMP',		        'COMPROBANTES',	        1, 3),
    ('COMPRADOR',		    'COMPRADOR',            1, 1),
    ('COMPRAS',		        'COMPRAS',		        1, 1),
    ('COMPRASADMIN',		'COMPRAS ADMIN',	    1, 1),
    ('CORR',		        'CORREDOR',		        1, 2),
    ('CRDECPE',		        'CESIÓN Y RECTIFICACIÓN DE CPE', 1, 3),
    ('DDAG',		        'DESHABILITADO EN DATAAGRO', 1, 2),
    ('DES',		            'DESHABILITADO',	    1, 2),
    ('DISCAL',		        'DISCREPANCIA CALIDADES', 1, 3),
    ('ECHEQ ADMIN',		    'GESTION ECHEQ ADMIN',	1, 1),
    ('FASON ADMIN',		    'FASON ADMIN',	        1, 1),
    ('FASON',		        'FASON',		        1, 2),
    ('FINCOR',		        'FINAL CORREDOR',	    1, 3),
    ('FINDIR',		        'FINAL DIRECTO',	    1, 3),
    ('FLECONSULTA',	        'FLETES CONSULTA',	    1, 3),
    ('FLETE MOA',	        'FLETE MOA',            1, 1),
    ('FLETE',		        'FLETES',		        1, 2),
    ('FWEB',		        'FUNCIONAMIENTO WEB',	1, 3),
    ('GRAN',		        'GRANOS',		        1, 2),
    ('GRANDA',		        'GRANOS + DATAAGRO',	1, 2),
    ('GYNG',		        'GRANOS Y NO GRANOS',	1, 2),
    ('GYNGDA',		        'GRANOS Y NO GRANOS + DATAAGRO', 1, 2),
    ('GYNGF',		        'GRANOS Y NO GRANOS + FLETE', 1, 2),
    ('GYNGP',		        'GRANOS Y NO GRANOS +PESIF TEST', 1, 2),
    ('MATBA',		        'OPERACIONES MATBA',	1, 3),
    ('MESAFAS',		        'MESA FAS',		        1, 2),
    ('MF',		            'MULTIFIRMA',	        1, 2),
    ('NOGRAN',		        'NO GRANOS',	        1, 2),
    ('NOIMP',		        'USUARIO NO IMPLEMENTADO', 1, 2),
    ('NUECLI',		        'NUEVO CLIENTE',	    1, 2),
    ('NUECORR',		        'NUEVO CORREDOR',	    1, 2),
    ('NUEG',		        'NUEVO USUARIO GRANOS',	1, 2),
    ('NUENOGRAN',		    'NUEVO USUARIO NO GRANOS', 1, 2),
    ('OPE',		            'OPERADOR',		        1, 1),
    ('ORD',		            'ORDEN DE CARGA',	    1, 3),
    ('OTRO',		        'OTROS',		        1, 3),
    ('PAG',		            'PAGOS',		        1, 3),
    ('PARCOR',		        'PARCIAL CORREDOR',	    1, 3),
    ('PARDIR',		        'PARCIAL DIRECTO',	    1, 3),
    ('PES',		            'PESIFICACIONES',	    1, 3),
    ('PROVG',		        'PROVEEDOR GENERAL CONSULTA', 1, 3),
    ('PUERTO',		        'PUERTO',		        1, 2),
    ('REI',		            'RECLAMO IMPOSITIVO',	1, 3),
    ('RESIDUOS ADMIN',		'RESIDUOS ADMIN',	    1, 1),
    ('RESIDUOS',	        'RESIDUOS',	        1, 2),
    ('RYDA',		        'RYD ADMINISTRACION',	1, 2),
    ('RYDU',		        'RYD USUARIO',	        1, 2),
    ('SOLP',		        'SOLP',		            1, 1),
    ('TODOS',		        'TODOS',		        1, 1)

INSERT INTO Rol
    (Codigo, Nombre, EsEditable, TipoRol)
SELECT
    V.Codigo, V.Nombre, V.EsEditable, V.TipoRol
FROM
    @ValoresRol V LEFT JOIN
    Rol R ON V.Codigo = R.Codigo
WHERE R.Codigo IS NULL


-- Actualizar solo si el TipoRol actual es 0
UPDATE R
SET R.TipoRol = V.TipoRol
FROM Rol R
INNER JOIN @ValoresRol V ON R.Codigo = V.Codigo
WHERE R.TipoRol = 0

-- DELETES
DELETE Rol WHERE Codigo = 'APLCLICPEDG' AND Nombre = 'CLIENTE CON CPEDG' -- El rol 'CLIENTE CON CPEDG' deja de estar vigente

-- Verificar si existe el rol 'REPORTE FACTURAS CERTIFICACIONES' si no existe se crea
IF NOT EXISTS (SELECT TOP 1 1 FROM Rol WHERE Codigo = 'REPORTE FACTURAS CERTIFICACIONES')
BEGIN
    INSERT INTO Rol (Codigo, Nombre, EsEditable, TipoRol)
    VALUES ('REPORTE FACTURAS CERTIFICACIONES', 'REPORTE FACTURAS CERTIFICACIONES', 1, 1)
END

-- Verificar si existe el rol 'SOLICITANTE EXTERNO' si no existe se crea
IF NOT EXISTS (SELECT TOP 1 1 FROM Rol WHERE Codigo = 'SOLICITANTE EXTERNO')
BEGIN
    INSERT INTO Rol (Codigo, Nombre, EsEditable, TipoRol)
    VALUES ('SOLICITANTE EXTERNO', 'SOLICITANTE EXTERNO', 1, 2)
END


