
--USE Moaoperaciones
--GO

--BEGIN TRAN 
---- OBTIENE el Id del Permiso: ABM SOLP
--declare @PermisoPorRol_Id int
--select @PermisoPorRol_Id = Id
--from [dbo].[PermisoPorRol]
--where Permiso = 'ABM SOLP'

---- LISTA los roles que tienen el permiso: ABM SOLP
----NOTA: Obtener la lista antes de eliminar y generar un script para cuando se quiera poner de vuelta el permiso a todos los ROLes que lo tenian
--/*
--insert into RolPermisoPorRol (Rol_Id, PermisoPorRol_Id) values (1, 90) 
--insert into RolPermisoPorRol (Rol_Id, PermisoPorRol_Id) values (54, 90) 
--insert into RolPermisoPorRol (Rol_Id, PermisoPorRol_Id) values (55, 90) 
--*/
--select *
--from [dbo].[RolPermisoPorRol]
--where PermisoPorRol_Id = @PermisoPorRol_Id



---- Eliminar el permiso: ABM SOLP de todos los ROLes que lo contengan
--/*
--delete [dbo].[RolPermisoPorRol]
--where PermisoPorRol_Id = @PermisoPorRol_Id

--ROLLBACK
----COMMIT