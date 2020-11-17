insert into dbo.RolPermisoPorRol values ((SELECT Id FROM dbo.Rol where Codigo = 'MF'), (select Id FROM dbo.PermisoPorRol where Permiso = 'CONSULTAR VENDEDORES'));
