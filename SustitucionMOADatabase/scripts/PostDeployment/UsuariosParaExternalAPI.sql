IF NOT EXISTS (SELECT TOP 1 1 FROM Usuario WHERE Mail = 'scato.logistica@molinosagro.com.ar') 
BEGIN
	INSERT INTO Usuario (Mail,CUITRegistro, Habilitado,TipoUsuario_Id, ApiKey) VALUES ('scato.logistica@molinosagro.com.ar','30715118773',0,5,'7cy0vWHYItVFd6phbcz1Id+qcUWVLYgwYLdJ8ZSq2xtrkxkMbQphs+oXG0/bjzgV');
END

IF NOT EXISTS (SELECT TOP 1 1 FROM RolUsuario WHERE Rol_Id =(SELECT Id FROM Rol WHERE Rol.Nombre='API ORDENES DE CARGA') and
	Usuario_Id=(SELECT Id FROM Usuario WHERE Mail='scato.logistica@molinosagro.com.ar')) 
BEGIN
	INSERT INTO RolUsuario VALUES ((SELECT Id FROM Rol WHERE Rol.Nombre='API ORDENES DE CARGA'),(SELECT Id FROM Usuario WHERE Mail='scato.logistica@molinosagro.com.ar'));
END

