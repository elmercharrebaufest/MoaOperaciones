INSERT INTO QRCamionesConfiguracion (NombreEtapa, TiempoEstimado, TipoWorkflow, FinEtapa, FinEtapaEsControlRecorrido)
VALUES
('Ingreso', 30, 'Granos', 'Crear Carta Porte', 1),
('Pre Calado', 45, 'Granos', 'Calado', 0),
('Calado', 120, 'Granos', 'Calado', 1),
('Post Calado', 60, 'Granos', 'Playa Externa', 1),
('Pesaje Bruto', 120, 'Granos', 'Pesada (Bruto)', 1),
('Descarga', 45, 'Granos', 'Descarga', 1),
('Cierre', 30, 'Granos', 'Fin de Workflow', 1);