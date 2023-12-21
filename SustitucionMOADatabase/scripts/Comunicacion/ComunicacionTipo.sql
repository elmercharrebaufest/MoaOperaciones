-- Llena tabla ComunicacionTipo
INSERT INTO dbo.ComunicacionTipo (Id, Descripcion)
SELECT 1, 'Exenciones Vencidas'
WHERE NOT EXISTS (SELECT 1 FROM dbo.ComunicacionTipo WHERE Id = 1);

INSERT INTO dbo.ComunicacionTipo (Id, Descripcion)
SELECT 2, 'Exenciones a Vencer'
WHERE NOT EXISTS (SELECT 1 FROM dbo.ComunicacionTipo WHERE Id = 2);

INSERT INTO dbo.ComunicacionTipo (Id, Descripcion)
SELECT 3, 'CM05 Vencido'
WHERE NOT EXISTS (SELECT 1 FROM dbo.ComunicacionTipo WHERE Id = 3);

INSERT INTO dbo.ComunicacionTipo (Id, Descripcion)
SELECT 4, 'Cuentas Habilitadas'
WHERE NOT EXISTS (SELECT 1 FROM dbo.ComunicacionTipo WHERE Id = 4);

INSERT INTO dbo.ComunicacionTipo (Id, Descripcion)
SELECT 5, 'Comunicaciones Usuarios'
WHERE NOT EXISTS (SELECT 1 FROM dbo.ComunicacionTipo WHERE Id = 5);

INSERT INTO dbo.ComunicacionTipo (Id, Descripcion)
SELECT 6, 'Liquidaciones Observadas'
WHERE NOT EXISTS (SELECT 1 FROM dbo.ComunicacionTipo WHERE Id = 6);
