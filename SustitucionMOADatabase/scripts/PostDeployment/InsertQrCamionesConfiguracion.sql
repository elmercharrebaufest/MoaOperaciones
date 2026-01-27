IF NOT EXISTS (SELECT 1 FROM QRCamionesConfiguracion WHERE NombreEtapa = 'Ingreso')
BEGIN
    INSERT INTO QRCamionesConfiguracion (NombreEtapa, TiempoEstimado, TipoWorkflow, FinEtapa, FinEtapaEsControlRecorrido)
    VALUES ('Ingreso', 30, 'Ingreso por Compra de Granos - Calada Externa', 'Crear Carta Porte', 0);
END;

IF NOT EXISTS (SELECT 1 FROM QRCamionesConfiguracion WHERE NombreEtapa = 'Pre Calado')
BEGIN
    INSERT INTO QRCamionesConfiguracion (NombreEtapa, TiempoEstimado, TipoWorkflow, FinEtapa, FinEtapaEsControlRecorrido)
    VALUES ('Pre Calado', 45, 'Ingreso por Compra de Granos - Calada Externa', 'Calado', 0);
END;

IF NOT EXISTS (SELECT 1 FROM QRCamionesConfiguracion WHERE NombreEtapa = 'Calado')
BEGIN
    INSERT INTO QRCamionesConfiguracion (NombreEtapa, TiempoEstimado, TipoWorkflow, FinEtapa, FinEtapaEsControlRecorrido)
    VALUES ('Calado', 120, 'Ingreso por Compra de Granos - Calada Externa', 'Calado', 1);
END;

IF NOT EXISTS (SELECT 1 FROM QRCamionesConfiguracion WHERE NombreEtapa = 'Post Calado')
BEGIN
    INSERT INTO QRCamionesConfiguracion (NombreEtapa, TiempoEstimado, TipoWorkflow, FinEtapa, FinEtapaEsControlRecorrido)
    VALUES ('Post Calado', 60, 'Ingreso por Compra de Granos - Calada Externa', 'En Playa Externa', 1);
END;

IF NOT EXISTS (SELECT 1 FROM QRCamionesConfiguracion WHERE NombreEtapa = 'Pesaje Bruto')
BEGIN
    INSERT INTO QRCamionesConfiguracion (NombreEtapa, TiempoEstimado, TipoWorkflow, FinEtapa, FinEtapaEsControlRecorrido)
    VALUES ('Pesaje Bruto', 120, 'Ingreso por Compra de Granos - Calada Externa', 'Pesada (Bruto)', 1);
END;

IF NOT EXISTS (SELECT 1 FROM QRCamionesConfiguracion WHERE NombreEtapa = 'Descarga')
BEGIN
    INSERT INTO QRCamionesConfiguracion (NombreEtapa, TiempoEstimado, TipoWorkflow, FinEtapa, FinEtapaEsControlRecorrido)
    VALUES ('Descarga', 45, 'Ingreso por Compra de Granos - Calada Externa', 'Confirmación de Carga/Descarga', 1);
END;

IF NOT EXISTS (SELECT 1 FROM QRCamionesConfiguracion WHERE NombreEtapa = 'Cierre')
BEGIN
    INSERT INTO QRCamionesConfiguracion (NombreEtapa, TiempoEstimado, TipoWorkflow, FinEtapa, FinEtapaEsControlRecorrido)
    VALUES ('Cierre', 30, 'Ingreso por Compra de Granos - Calada Externa', 'Fin De Workflow', 0);
END;