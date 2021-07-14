BEGIN TRANSACTION;

	BEGIN
		insert into PermisoPorRol values ('CONSULTA ABM')
	END

	DECLARE @ConsultaABM INT
	DECLARE @ContactoMail INT
	set @ConsultaABM = (select Id from PermisoPorRol where Permiso = 'CONSULTA ABM')
	set @ContactoMail = (select Id from PermisoPorRol where Permiso = 'CONTACTO MAIL')

	BEGIN
		insert into Rol values ('BOL', 'BOLETOS', 1)
		insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'BOL'), @ConsultaABM)
		insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'BOL'), @ContactoMail)
		insert into CategoriaRol values ((select Id from Rol where Codigo = 'BOL'), (select Id from Categoria where Code = 'BOL'))
	END

	BEGIN
		insert into Rol values ('REI', 'RECLAMO IMPOSITIVO', 1)
		insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'REI'), @ConsultaABM)
		insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'REI'), @ContactoMail)
		insert into CategoriaRol values ((select Id from Rol where Codigo = 'REI'), (select Id from Categoria where Code = 'REI'))
	END

	BEGIN
		insert into Rol values ('ACT', 'ACTUALIZACIÓN', 1)
		insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'ACT'), @ConsultaABM)
		insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'ACT'), @ContactoMail)
		insert into CategoriaRol values ((select Id from Rol where Codigo = 'ACT'), (select Id from Categoria where Code = 'ACT'))
	END

	BEGIN
		insert into Rol values ('PARDIR', 'PARCIAL DIRECTO', 1)
		insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'PARDIR'), @ConsultaABM)
		insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'PARDIR'), @ContactoMail)
		insert into CategoriaRol values ((select Id from Rol where Codigo = 'PARDIR'), (select Id from Categoria where Code = 'PARDIR'))
	END

	BEGIN
		insert into Rol values ('PARCOR', 'PARCIAL CORREDOR', 1)
		insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'PARCOR'), @ConsultaABM)
		insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'PARCOR'), @ContactoMail)
		insert into CategoriaRol values ((select Id from Rol where Codigo = 'PARCOR'), (select Id from Categoria where Code = 'PARCOR'))
	END
	
	BEGIN
		insert into Rol values ('FINDIR', 'FINAL DIRECTO', 1)
		insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'FINDIR'), @ConsultaABM)
		insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'FINDIR'), @ContactoMail)
		insert into CategoriaRol values ((select Id from Rol where Codigo = 'FINDIR'), (select Id from Categoria where Code = 'FINDIR'))
	END
	
	BEGIN
		insert into Rol values ('FINCOR', 'FINAL CORREDOR', 1)
		insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'FINCOR'), @ConsultaABM)
		insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'FINCOR'), @ContactoMail)
		insert into CategoriaRol values ((select Id from Rol where Codigo = 'FINCOR'), (select Id from Categoria where Code = 'FINCOR'))
	END
	
	BEGIN
		insert into Rol values ('CAL', 'CALIDADES', 1)
		insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'CAL'), @ConsultaABM)
		insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'CAL'), @ContactoMail)
		insert into CategoriaRol values ((select Id from Rol where Codigo = 'CAL'), (select Id from Categoria where Code = 'CAL'))
	END

	BEGIN
		insert into Rol values ('COM', 'COMISIONES', 1)
		insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'COM'), @ConsultaABM)
		insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'COM'), @ContactoMail)
		insert into CategoriaRol values ((select Id from Rol where Codigo = 'COM'), (select Id from Categoria where Code = 'COM'))
	END

	BEGIN
		insert into Rol values ('COMP', 'COMPROBANTES', 1)
		insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'COMP'), @ConsultaABM)
		insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'COMP'), @ContactoMail)
		insert into CategoriaRol values ((select Id from Rol where Codigo = 'COMP'), (select Id from Categoria where Code = 'COMP'))
	END

	BEGIN
		insert into Rol values ('APP', 'APLICACIONES', 1)
		insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'APP'), @ConsultaABM)
		insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'APP'), @ContactoMail)
		insert into CategoriaRol values ((select Id from Rol where Codigo = 'APP'), (select Id from Categoria where Code = 'APP'))
	END
	
	BEGIN
		insert into Rol values ('PES', 'PESIFICACIONES', 1)
		insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'PES'), @ConsultaABM)
		insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'PES'), @ContactoMail)
		insert into CategoriaRol values ((select Id from Rol where Codigo = 'PES'), (select Id from Categoria where Code = 'PES'))
	END

	
	BEGIN
		insert into Rol values ('PAG', 'PAGOS', 1)
		insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'PAG'), @ConsultaABM)
		insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'PAG'), @ContactoMail)
		insert into CategoriaRol values ((select Id from Rol where Codigo = 'PAG'), (select Id from Categoria where Code = 'PAG'))
	END
	
	BEGIN
		insert into Rol values ('FWEB', 'FUNCIONAMIENTO WEB', 1)
		insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'FWEB'), @ConsultaABM)
		insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'FWEB'), @ContactoMail)
		insert into CategoriaRol values ((select Id from Rol where Codigo = 'FWEB'), (select Id from Categoria where Code = 'FWEB'))
	END

	BEGIN
		insert into Rol values ('MATBA', 'OPERACIONES MATBA', 1)
		insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'MATBA'), @ConsultaABM)
		insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'MATBA'), @ContactoMail)
		insert into CategoriaRol values ((select Id from Rol where Codigo = 'MATBA'), (select Id from Categoria where Code = 'MATBA'))
	END

	BEGIN
		insert into Rol values ('PROVGC', 'ROVEEDOR GENERAL CONSULTA', 1)
		insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'PROVGC'), @ConsultaABM)
		insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'PROVGC'), @ContactoMail)
		insert into CategoriaRol values ((select Id from Rol where Codigo = 'PROVGC'), (select Id from Categoria where Code = 'PROVG'))
	END

	BEGIN
		insert into Rol values ('FLECONSULTA', 'FLETES CONSULTA', 1)
		insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'FLECONSULTA'), @ConsultaABM)
		insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'FLECONSULTA'), @ContactoMail)
		insert into CategoriaRol values ((select Id from Rol where Codigo = 'FLECONSULTA'), (select Id from Categoria where Code = 'FLET'))
	END

	BEGIN
		insert into Rol values ('OTRO', 'OTROS', 1)
		insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'OTRO'), @ConsultaABM)
		insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'OTRO'), @ContactoMail)
		insert into CategoriaRol values ((select Id from Rol where Codigo = 'OTRO'), (select Id from Categoria where Code = 'OTRO'))
	END

COMMIT TRANSACTION;