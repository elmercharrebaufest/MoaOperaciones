BEGIN TRANSACTION;
insert into PermisoPorRol values ('ALTA EMPRESA NO GRANOS')
insert into rol values ('NUENOGRAN','NUEVO USUARIO NO GRANOS',1)

declare @rolid int;
select @rolid= id from rol where codigo = 'NUENOGRAN'

declare @permiso1 int;
select @permiso1= id from PermisoPorRol where Permiso = 'CONTACTO MAIL'
declare @permiso2 int;
select @permiso2= id from PermisoPorRol where Permiso = 'ESTADO SOLICITUD'
declare @permiso3 int;
select @permiso3= id from PermisoPorRol where Permiso = 'ALTA EMPRESA NO GRANOS'

insert into RolPermisoPorRol values (@rolid,@permiso1)
insert into RolPermisoPorRol values (@rolid,@permiso2)
insert into RolPermisoPorRol values (@rolid,@permiso3)

select * from rol where Id = @rolid
select * from PermisoPorRol where Id  =@permiso3
select * from RolPermisoPorRol where Rol_Id = @rolid
COMMIT;