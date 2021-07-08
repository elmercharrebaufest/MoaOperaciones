---Para los que estan en estado aprobado
UPDATE 
	dbo.Proveedor
SET	
	ContieneDocumentacionFisica = 1
WHERE
	EstadoAprobacion = 0 AND
	TipoProveedor_Id = 3

---Para los restantes distintos a aprobados
UPDATE 
	dbo.Proveedor
SET	
	ContieneDocumentacionFisica = 0
WHERE
	EstadoAprobacion <> 0 AND
	TipoProveedor_Id = 3


