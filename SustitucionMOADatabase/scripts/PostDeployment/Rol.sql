--Rol
--select *,'IF NOT EXISTS(SELECT 1 FROM Rol WHERE Codigo = '''+Codigo+''' AND Nombre = '''+Nombre+''') BEGIN INSERT INTO Rol values ('''+Codigo+''','''+Nombre+''','+ltrim(EsEditable)+')'from rol

-- UPDATES
UPDATE Rol SET Codigo = 'PROVG' WHERE Codigo = 'PROVGC'
UPDATE Rol SET Nombre = 'CERTIFICACIÓN DE SERVICIOS EXT' WHERE Codigo = 'CERTIFICACION EXTERNA'


-- TABLA DE ROLES
DECLARE @ValoresRol as TABLE
    (Codigo varchar(max), Nombre varchar(max), EsEditable bit)

INSERT INTO @ValoresRol
VALUES
    ('ACCESO QR',		    'Acceso QR',		1),
    ('ACT',		            'ACTUALIZACIÓN',		1),
    ('ADM',		            'ADMINISTRACION',		1),
    ('ADMCM05',		        'Gestion CM05',		1),
    ('ADMIN_CURSOS',        'CURSOS ADMIN',     1),
    ('ADMINCCSS',		    'ADMIN CAMPO SUSTENTABLE',		1),
    ('ADMINCONTMA',		    'ADMIN CONTABILIZACION MES ANTERIOR',		1),
    ('ADMINPLATCOMPRAS',    'ADMIN PLATAFORMA COMPRAS',		1),
    ('ADU',		            'ADUANA',		1),
    ('AIGRAN',		        'ALTA INTERNA GRANOS',		1),
    ('AINOGRAN',	        'ALTA INTERNA NO GRANOS',		1),
    ('ALLES',		        'VER TODOS LOS ESTADOS DE ES',		1),
    ('ALUMNO_CURSOS',       'ALUMNO CURSOS',        1),
    ('ANUL',		        'ANULADOR',		1),
    ('API_ORDCARGA',        'API ORDENES DE CARGA',     0),
    ('API_ORDRESIDUOS',     'API ORDENES RESIDUOS',     0),
    ('APIKEY',		        'APIKEY',		1),
    ('APLCCPP ADMIN',		'APLICACION CCPP ADMIN',		1),
    ('APLCCPP',		        'APLICACION CCPP',		1),
    ('APP',		            'APLICACIONES',		1),
    ('APRO',		        'APROBADOR',		1),
    ('AUDITOR COMPRAS',		'AUDITOR COMPRAS',		1),
    ('BOL',		            'BOLETOS',		1),
    ('CAL',		            'CALIDADES',		1),
    ('CERTIFICACION',       'CERTIFICACIÓN DE SERVICIOS',      1),
    ('CERTIFICACION EXTERNA','CERTIFICACIÓN DE SERVICIOS EXT',     1),
    ('CLIENT',	            'SOLO CLIENTES',		1),
    ('COM',		            'COMISIONES',		1),
    ('COMERCIAL',		    'COMERCIAL',		1),
    ('COMP',		        'COMPROBANTES',		1),
    ('COMPRADOR',		    'COMPRADOR', 1),
    ('COMPRAS',		        'COMPRAS',		1), 
    ('COMPRASADMIN',		'COMPRAS ADMIN',		1),
    ('CORR',		        'CORREDOR',		1),
    ('CRDECPE',		        'CESIÓN Y RECTIFICACIÓN DE CPE',		1),
    ('DDAG',		        'DESHABILITADO EN DATAAGRO',		1),
    ('DES',		            'DESHABILITADO',		1),
    ('DISCAL',		        'DISCREPANCIA CALIDADES',		1),
    ('ECHEQ ADMIN',		    'GESTION ECHEQ ADMIN',		1),
    ('FASON ADMIN',		    'FASON ADMIN',		1),
    ('FASON',		        'FASON',		1),
    ('FINCOR',		        'FINAL CORREDOR',		1),
    ('FINDIR',		        'FINAL DIRECTO',		1),
    ('FLECONSULTA',	        'FLETES CONSULTA',		1),
    ('FLETE MOA',	        'FLETE MOA', 1),
    ('FLETE',		        'FLETES',		1),
    ('FWEB',		        'FUNCIONAMIENTO WEB',		1),
    ('GRAN',		        'GRANOS',		1),
    ('GRANDA',		        'GRANOS + DATAAGRO',		1),
    ('GYNG',		        'GRANOS Y NO GRANOS',		1),
    ('GYNGDA',		        'GRANOS Y NO GRANOS + DATAAGRO',		1),
    ('GYNGF',		        'GRANOS Y NO GRANOS + FLETE',		1),
    ('GYNGP',		        'GRANOS Y NO GRANOS +PESIF TEST',		1),
    ('MATBA',		        'OPERACIONES MATBA',		1),
    ('MESAFAS',		        'MESA FAS',		1),
    ('MF',		            'MULTIFIRMA',		1),
    ('NOGRAN',		        'NO GRANOS',		1),
    ('NOIMP',		        'USUARIO NO IMPLEMENTADO',		1),
    ('NUECLI',		        'NUEVO CLIENTE',		1),
    ('NUECORR',		        'NUEVO CORREDOR',		1),
    ('NUEG',		        'NUEVO USUARIO GRANOS',		1),
    ('NUENOGRAN',		    'NUEVO USUARIO NO GRANOS',		1),
    ('OPE',		            'OPERADOR',		1),
    ('ORD',		            'ORDEN DE CARGA',		1),
    ('OTRO',		        'OTROS',		1),
    ('PAG',		            'PAGOS',		1),
    ('PARCOR',		        'PARCIAL CORREDOR',		1),
    ('PARDIR',		        'PARCIAL DIRECTO',		1),
    ('PES',		            'PESIFICACIONES',		1),
    ('PROVG',		        'PROVEEDOR GENERAL CONSULTA',		1),
    ('PUERTO',		        'PUERTO',		1),
    ('REI',		            'RECLAMO IMPOSITIVO',		1),
    ('RESIDUOS ADMIN',		'RESIDUOS ADMIN',		1),
    ('RESIDUOS',	        'RESIDUOS',		1),
    ('RYDA',		        'RYD ADMINISTRACION',		1),
    ('RYDU',		        'RYD USUARIO',		1),
    ('SOLP',		        'SOLP',		1),
    ('TODOS',		        'TODOS',		1)


INSERT INTO Rol
    (Codigo, Nombre, EsEditable)
SELECT
    V.Codigo, V.Nombre, V.EsEditable
FROM
    @ValoresRol V left join
    Rol R on V.Codigo = R.Codigo
WHERE R.Codigo is null


-- DELETES
DELETE Rol WHERE Codigo = 'APLCLICPEDG' AND Nombre = 'CLIENTE CON CPEDG' -- El rol 'CLIENTE CON CPEDG' deja de estar vigente


