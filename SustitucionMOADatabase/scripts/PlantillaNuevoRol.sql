BEGIN TRANSACTION;

	insert into PermisoPorRol values ('CONSULTA ABM')
	insert into RolPermisoPorRol values (1, (select Id from PermisoPorRol where Permiso = 'CONSULTA ABM'))

	--insert into PermisoPorRol values ('CONSULTA ACTUALIZACION')
	--insert into PermisoPorRol values ('CONSULTA RECLAMO I')
	--insert into PermisoPorRol values ('CONSULTA BOLETOS')


	--TENGO QUE VER COMO SE VA A LLAMAR ESTE ROL
	insert into rol values ('', '', 1)
	insert into RolPermisoPorRol values ((select Id from Rol where Codigo = ''), (select Id from PermisoPorRol where Permiso = 'CONSULTA ABM'))

	insert into CategoriaRol values ((select Id from Rol where Codigo = ''), (select Id from Categoria where Code = 'REI'))
	insert into CategoriaRol values ((select Id from Rol where Codigo = ''), (select Id from Categoria where Code = 'ACT'))
	insert into CategoriaRol values ((select Id from Rol where Codigo = ''), (select Id from Categoria where Code = 'BOL'))

COMMIT TRANSACTION;