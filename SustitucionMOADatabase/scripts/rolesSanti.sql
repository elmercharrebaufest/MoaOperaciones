BEGIN TRANSACTION;

	insert into Rol values ('PARDIR', 'PARCIAL DIRECTO', 1)
	insert into Rol values ('PARCOR', 'PARCIAL CORREDOR', 1)
	insert into Rol values ('FINDIR', 'FINAL DIRECTO', 1)
	insert into Rol values ('FINCOR', 'FINAL CORREDOR', 1)
	insert into Rol values ('CAL', 'CALIDADES', 1)
	insert into Rol values ('COM', 'COMISIONES', 1)
	insert into Rol values ('COMP', 'COMPROBANTES', 1)
	insert into Rol values ('APP', 'APLICACIONES', 1)
	insert into Rol values ('PES', 'PESIFICACIONES', 1)
	insert into Rol values ('PAG', 'PAGOS', 1)
	insert into Rol values ('FWEB', 'FUNCIONAMIENTO WEB', 1)
	insert into Rol values ('MATBA', 'OPERACIONES MATBA', 1)
	insert into Rol values ('PROVGC', 'ROVEEDOR GENERAL CONSULTA', 1)
	insert into Rol values ('FLECONSULTA', 'FLETES CONSULTA', 1)
	insert into Rol values ('OTRO', 'OTROS', 1)

	insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'PARDIR'), (select Id from PermisoPorRol where Permiso = 'CONSULTA ABM'))
	insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'PARDIR'), (select Id from PermisoPorRol where Permiso = 'CONTACTO MAIL'))

	insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'PARCOR'), (select Id from PermisoPorRol where Permiso = 'CONSULTA ABM'))
	insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'PARCOR'), (select Id from PermisoPorRol where Permiso = 'CONTACTO MAIL'))

	insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'FINDIR'), (select Id from PermisoPorRol where Permiso = 'CONSULTA ABM'))
	insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'FINDIR'), (select Id from PermisoPorRol where Permiso = 'CONTACTO MAIL'))

	insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'FINCOR'), (select Id from PermisoPorRol where Permiso = 'CONSULTA ABM'))
	insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'FINCOR'), (select Id from PermisoPorRol where Permiso = 'CONTACTO MAIL'))

	insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'CAL'), (select Id from PermisoPorRol where Permiso = 'CONSULTA ABM'))
	insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'CAL'), (select Id from PermisoPorRol where Permiso = 'CONTACTO MAIL'))

	insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'COM'), (select Id from PermisoPorRol where Permiso = 'CONSULTA ABM'))
	insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'COM'), (select Id from PermisoPorRol where Permiso = 'CONTACTO MAIL'))

	insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'COMP'), (select Id from PermisoPorRol where Permiso = 'CONSULTA ABM'))
	insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'COMP'), (select Id from PermisoPorRol where Permiso = 'CONTACTO MAIL'))

	insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'APP'), (select Id from PermisoPorRol where Permiso = 'CONSULTA ABM'))
	insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'APP'), (select Id from PermisoPorRol where Permiso = 'CONTACTO MAIL'))

	insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'PES'), (select Id from PermisoPorRol where Permiso = 'CONSULTA ABM'))
	insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'PES'), (select Id from PermisoPorRol where Permiso = 'CONTACTO MAIL'))

	insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'PAG'), (select Id from PermisoPorRol where Permiso = 'CONSULTA ABM'))
	insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'PAG'), (select Id from PermisoPorRol where Permiso = 'CONTACTO MAIL'))

	insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'FWEB'), (select Id from PermisoPorRol where Permiso = 'CONSULTA ABM'))
	insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'FWEB'), (select Id from PermisoPorRol where Permiso = 'CONTACTO MAIL'))

	insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'MATBA'), (select Id from PermisoPorRol where Permiso = 'CONSULTA ABM'))
	insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'MATBA'), (select Id from PermisoPorRol where Permiso = 'CONTACTO MAIL'))

	insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'PROVGC'), (select Id from PermisoPorRol where Permiso = 'CONSULTA ABM'))
	insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'PROVGC'), (select Id from PermisoPorRol where Permiso = 'CONTACTO MAIL'))

	insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'FLECONSULTA'), (select Id from PermisoPorRol where Permiso = 'CONSULTA ABM'))
	insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'FLECONSULTA'), (select Id from PermisoPorRol where Permiso = 'CONTACTO MAIL'))

	insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'OTRO'), (select Id from PermisoPorRol where Permiso = 'CONSULTA ABM'))
	insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'OTRO'), (select Id from PermisoPorRol where Permiso = 'CONTACTO MAIL'))

	insert into CategoriaRol values ((select Id from Rol where Codigo = 'PARDIR'), (select Id from Categoria where Code = 'PARDIR'))
	insert into CategoriaRol values ((select Id from Rol where Codigo = 'FINDIR'), (select Id from Categoria where Code = 'FINDIR'))
	insert into CategoriaRol values ((select Id from Rol where Codigo = 'PARCOR'), (select Id from Categoria where Code = 'PARCOR'))
	insert into CategoriaRol values ((select Id from Rol where Codigo = 'FINCOR'), (select Id from Categoria where Code = 'FINCOR'))
	insert into CategoriaRol values ((select Id from Rol where Codigo = 'CAL'), (select Id from Categoria where Code = 'CAL'))
	insert into CategoriaRol values ((select Id from Rol where Codigo = 'COM'), (select Id from Categoria where Code = 'COM'))
	insert into CategoriaRol values ((select Id from Rol where Codigo = 'COMP'), (select Id from Categoria where Code = 'COMP'))
	insert into CategoriaRol values ((select Id from Rol where Codigo = 'APP'), (select Id from Categoria where Code = 'APP'))
	insert into CategoriaRol values ((select Id from Rol where Codigo = 'PES'), (select Id from Categoria where Code = 'PES'))
	insert into CategoriaRol values ((select Id from Rol where Codigo = 'PAG'), (select Id from Categoria where Code = 'PAG'))
	insert into CategoriaRol values ((select Id from Rol where Codigo = 'FWEB'), (select Id from Categoria where Code = 'FWEB'))
	insert into CategoriaRol values ((select Id from Rol where Codigo = 'MATBA'), (select Id from Categoria where Code = 'MATBA'))
	insert into CategoriaRol values ((select Id from Rol where Codigo = 'PROVGC'), (select Id from Categoria where Code = 'PROVG'))
	insert into CategoriaRol values ((select Id from Rol where Codigo = 'FLECONSULTA'), (select Id from Categoria where Code = 'FLET'))
	insert into CategoriaRol values ((select Id from Rol where Codigo = 'OTRO'), (select Id from Categoria where Code = 'OTRO'))

COMMIT TRANSACTION;