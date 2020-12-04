BEGIN TRANSACTION;
insert into PermisoPorRol values ('ALTA EMPRESA NO GRANOS')
insert into rol values ('NUENOGRAN','NUEVO USUARIO NO GRANOS',1)

declare @rol2 int;
insert into rol values ('COMPRAS','COMPRAS',1)
set @rol2= @@IDENTITY

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
insert into RolPermisoPorRol values (@rol2,@permiso3)

insert into rubro values ('ACEITE')
insert into rubro values ('ADMINISTRACION')
insert into rubro values ('COMEX')
insert into rubro values ('ENERGIA')
insert into rubro values ('FLETES')
insert into rubro values ('HONORARIOS')
insert into rubro values ('IMP Y SERV')
insert into rubro values ('INSUMOS RESTO')
insert into rubro values ('MAQUINARIAS')
insert into rubro values ('MAQUINARIAS REPUESTOS')
insert into rubro values ('MARKETING')
insert into rubro values ('MATERIALES AUXILIARES')
insert into rubro values ('REPUESTOS')
insert into rubro values ('RRHH')
insert into rubro values ('SEGUROS')
insert into rubro values ('SERVICIOS CONTRATISTAS')
insert into rubro values ('SERVICIOS MANTENIMIENTO')
insert into rubro values ('SERVICIOS PERMANENTES')
insert into rubro values ('SISTEMAS')
insert into rubro values ('VARIOS')


COMMIT;

insert into IngresoBruto values ('Responsable No Inscripto')
insert into IngresoBruto values ('Exento')
insert into IngresoBruto values ('Monotributo')


insert into SituacionIVA values ('Responsable Inscripto')
insert into SituacionIVA values ('Responsable No Inscripto')
insert into SituacionIVA values ('Exento')
insert into SituacionIVA values ('Monotributo')
