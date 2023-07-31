BEGIN TRAN
IF EXISTS (SELECT TOP 1 1 FROM dbo.Usuario WHERE Mail='eliana.chocobar@molinosagro.com.ar') AND
NOT EXISTS (SELECT TOP 1 1 FROM dbo.RolUsuario WHERE Rol_Id=(SELECT Id FROM Rol Where Nombre='FLETE MOA')
    AND Usuario_Id=(SELECT Id FROM Usuario WHERE Mail='eliana.chocobar@molinosagro.com.ar'))
BEGIN
    INSERT INTO RolUsuario VALUES ((SELECT Id FROM Rol Where Nombre='FLETE MOA'),(SELECT Id FROM Usuario WHERE Mail='eliana.chocobar@molinosagro.com.ar'))
END
COMMIT TRAN

BEGIN TRAN
IF EXISTS (SELECT TOP 1 1 FROM dbo.Usuario WHERE Mail='jorgelina.franichevich@molinosagro.com.ar') AND
NOT EXISTS (SELECT TOP 1 1 FROM dbo.RolUsuario WHERE Rol_Id=(SELECT Id FROM Rol Where Nombre='FLETE MOA')
    AND Usuario_Id=(SELECT Id FROM Usuario WHERE Mail='jorgelina.franichevich@molinosagro.com.ar'))
BEGIN
INSERT INTO RolUsuario VALUES ((SELECT Id FROM Rol Where Nombre='FLETE MOA'),(SELECT Id FROM Usuario WHERE Mail='jorgelina.franichevich@molinosagro.com.ar'))
END
COMMIT TRAN
