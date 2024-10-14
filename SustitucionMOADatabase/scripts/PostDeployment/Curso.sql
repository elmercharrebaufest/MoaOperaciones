IF NOT EXISTS
	(SELECT 1 FROM Curso WHERE Nombre = 'Introducción a la Ciberseguridad') 
BEGIN INSERT INTO Curso (Nombre,Acceso, MinimosMinutosCursada)
VALUES('Introducción a la Ciberseguridad','Content/Cursos/Ciberseguridad/index_lms.html', 10) END 