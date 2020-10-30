UPDATE 
	P
SET	
	P.Comercial = UG.Comercial
FROM 
	dbo.Proveedor P
	INNER JOIN dbo.ProveedorUsuario PU ON PU.Proveedor_Id = P.Id
	INNER JOIN dbo.Usuario U ON U.Id = PU.Usuario_Id
	INNER JOIN dbo.UsuarioGranos UG ON UG.Id = U.Id

