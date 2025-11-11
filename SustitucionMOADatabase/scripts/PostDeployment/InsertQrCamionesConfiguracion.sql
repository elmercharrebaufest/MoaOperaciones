IF NOT EXISTS (SELECT 1 FROM QRCamionesConfiguracion WHERE NombreEtapa = 'Ingreso')
BEGIN
    INSERT INTO QRCamionesConfiguracion (NombreEtapa, TiempoEstimado, TipoWorkflow, FinEtapa, FinEtapaEsControlRecorrido)
    VALUES ('Ingreso', 30, 'Granos', 'Crear Carta Porte', 1);
END;

IF NOT EXISTS (SELECT 1 FROM QRCamionesConfiguracion WHERE NombreEtapa = 'Pre Calado')
BEGIN
    INSERT INTO QRCamionesConfiguracion (NombreEtapa, TiempoEstimado, TipoWorkflow, FinEtapa, FinEtapaEsControlRecorrido)
    VALUES ('Pre Calado', 45, 'Granos', 'Calado', 0);
END;

IF NOT EXISTS (SELECT 1 FROM QRCamionesConfiguracion WHERE NombreEtapa = 'Calado')
BEGIN
    INSERT INTO QRCamionesConfiguracion (NombreEtapa, TiempoEstimado, TipoWorkflow, FinEtapa, FinEtapaEsControlRecorrido)
    VALUES ('Calado', 120, 'Granos', 'Calado', 1);
END;

IF NOT EXISTS (SELECT 1 FROM QRCamionesConfiguracion WHERE NombreEtapa = 'Post Calado')
BEGIN
    INSERT INTO QRCamionesConfiguracion (NombreEtapa, TiempoEstimado, TipoWorkflow, FinEtapa, FinEtapaEsControlRecorrido)
    VALUES ('Post Calado', 60, 'Granos', 'Playa Externa', 1);
END;

IF NOT EXISTS (SELECT 1 FROM QRCamionesConfiguracion WHERE NombreEtapa = 'Pesaje Bruto')
BEGIN
    INSERT INTO QRCamionesConfiguracion (NombreEtapa, TiempoEstimado, TipoWorkflow, FinEtapa, FinEtapaEsControlRecorrido)
    VALUES ('Pesaje Bruto', 120, 'Granos', 'Pesada (Bruto)', 1);
END;

IF NOT EXISTS (SELECT 1 FROM QRCamionesConfiguracion WHERE NombreEtapa = 'Descarga')
BEGIN
    INSERT INTO QRCamionesConfiguracion (NombreEtapa, TiempoEstimado, TipoWorkflow, FinEtapa, FinEtapaEsControlRecorrido)
    VALUES ('Descarga', 45, 'Granos', 'Descarga', 1);
END;

IF NOT EXISTS (SELECT 1 FROM QRCamionesConfiguracion WHERE NombreEtapa = 'Cierre')
BEGIN
    INSERT INTO QRCamionesConfiguracion (NombreEtapa, TiempoEstimado, TipoWorkflow, FinEtapa, FinEtapaEsControlRecorrido)
    VALUES ('Cierre', 30, 'Granos', 'Fin de Workflow', 1);
END;