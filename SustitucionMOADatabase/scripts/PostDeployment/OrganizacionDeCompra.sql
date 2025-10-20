
IF NOT EXISTS (SELECT 1 FROM OrganizacionDeCompra WHERE Id = '2029') BEGIN INSERT INTO OrganizacionDeCompra (Id, Descripcion) VALUES ('2029', 'Estratégicas') END;
IF NOT EXISTS (SELECT 1 FROM OrganizacionDeCompra WHERE Id = '4010') BEGIN INSERT INTO OrganizacionDeCompra (Id, Descripcion) VALUES ('4010', 'RRHH') END;
