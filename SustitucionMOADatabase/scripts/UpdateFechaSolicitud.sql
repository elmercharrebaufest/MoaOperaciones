UPDATE 
	dbo.Proveedor
SET
--select Id, FechaSolicitud, (SELECT min(Fecha) from DBO.ProveedorHistorialAprobacion WHERE Proveedor_Id = Proveedor.Id) from Proveedor
	dbo.Proveedor.FechaSolicitud = (SELECT min(Fecha) from DBO.ProveedorHistorialAprobacion WHERE Proveedor_Id = Proveedor.Id)
	where Id != 39;
--Select * from ProveedorHistorialAprobacion order by Proveedor_Id, Fecha;

