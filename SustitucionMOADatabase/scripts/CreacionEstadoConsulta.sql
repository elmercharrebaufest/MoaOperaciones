SET NOCOUNT ON
BEGIN TRAN

BEGIN
	INSERT INTO EstadoConsulta(Code, Descripcion, Color)
	VALUES ('INI', 'Iniciado', '#d63838')

	INSERT INTO EstadoConsulta(Code, Descripcion, Color)
	VALUES ('GES', 'En Gestion', '#ec8b3b')

	INSERT INTO EstadoConsulta(Code, Descripcion, Color)
	VALUES ('DOC', 'Solicitud de información', '#9e9604')

	INSERT INTO EstadoConsulta(Code, Descripcion, Color)
	VALUES ('REC', 'Rechazado', '#ea3d3d')

	INSERT INTO EstadoConsulta(Code, Descripcion, Color)
	VALUES ('CER', 'Cerrado', '#6a6565')
END

COMMIT TRAN