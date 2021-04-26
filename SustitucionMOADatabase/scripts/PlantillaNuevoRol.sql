BEGIN TRANSACTION;

	insert into Rol values ('BOL', 'BOLETOS', 1)
	insert into Rol values ('REI', 'RECLAMO IMPOSITIVO', 1)
	insert into Rol values ('ACT', 'ACTUALIZACIÓN', 1)

	insert into PermisoPorRol values ('CONSULTA ABM')
	insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'BOL'), (select Id from PermisoPorRol where Permiso = 'CONSULTA ABM'))
	insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'ACT'), (select Id from PermisoPorRol where Permiso = 'CONSULTA ABM'))
	insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'REI'), (select Id from PermisoPorRol where Permiso = 'CONSULTA ABM'))

	insert into CategoriaRol values ((select Id from Rol where Codigo = 'ACT'), (select Id from Categoria where Code = 'ACT'))
	insert into CategoriaRol values ((select Id from Rol where Codigo = 'REI'), (select Id from Categoria where Code = 'REI'))
	insert into CategoriaRol values ((select Id from Rol where Codigo = 'BOL'), (select Id from Categoria where Code = 'BOL'))

COMMIT TRANSACTION;