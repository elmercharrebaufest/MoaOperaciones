IF NOT EXISTS (SELECT TOP 1 1 FROM Usuario WHERE Mail = 'scato@external.api.com') 
BEGIN
	INSERT INTO Usuario (Mail,CUITRegistro, Habilitado,TipoUsuario_Id, ApiKey) VALUES ('scato@external.api.com','30715118773',0,5,'kKeNTAR+A5KCAxGmWxbYj8zVh3umhDhllA3V1NVvYIO0Wr5TnEYJE6veE8mIhy6O');
	INSERT INTO RolUsuario VALUES ((SELECT Id FROM Rol WHERE Rol.Nombre='API ORDENES DE CARGA'),(SELECT Id FROM Usuario WHERE Mail='scato@external.api.com'));
END

