update EstadoConsulta
set Descripcion = 'Solicitud de información', Color = '#9e9604'
where Code = 'DOC' and Color = '#f2e92e'

begin tran

delete from CategoriaRol
where Rol_Id = (select Id from Rol where Codigo = 'DATMAE')

delete from rol
where Codigo = 'DATMAE'

insert into Rol values ('REI', 'RECLAMO IMPOSITIVO', 1)
insert into Rol values ('ACT', 'ACTUALIZACIÓN', 1)

insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'REI'), (select Id from PermisoPorRol where Permiso = 'CONSULTA ABM'))
insert into RolPermisoPorRol values ((select Id from Rol where codigo = 'ACT'), (select Id from PermisoPorRol where Permiso = 'CONSULTA ABM'))

insert into CategoriaRol values ((select Id from Rol where Codigo = 'REI'), (select Id from Categoria where Code = 'REI'))
insert into CategoriaRol values ((select Id from Rol where Codigo = 'ACT'), (select Id from Categoria where Code = 'ACT'))

commit tran