--LiberadorSapTipo

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.LiberadorSapTipo WHERE Nombre = 'SOLP') BEGIN INSERT INTO LiberadorSapTipo (Nombre) VALUES ('SOLP') END

--LiberadorSap

IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.LiberadorSap WHERE Mail = 'sergio.schoeder@molinosagro.com.ar' and NombreCompleto = 'Schoeder, Sergio' and Cargo = 'Jefe') 
BEGIN INSERT INTO LiberadorSap (Mail, NombreCompleto, Obligatorio, Habilitado, Cargo, LiberadorSapTipo_Id) VALUES ('sergio.schoeder@molinosagro.com.ar', 'Schoeder, Sergio', 0, 1, 'Jefe', 1) END
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.LiberadorSap WHERE Mail = 'hugo.baratto@molinosagro.com.ar' and NombreCompleto = 'Baratto, Hugo' and Cargo = 'Jefe') 
BEGIN INSERT INTO LiberadorSap (Mail, NombreCompleto, Obligatorio, Habilitado, Cargo, LiberadorSapTipo_Id) VALUES ('hugo.baratto@molinosagro.com.ar', 'Baratto, Hugo', 0, 1, 'Jefe', 1) END
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.LiberadorSap WHERE Mail = 'carlos.cortina@molinosagro.com.ar' and NombreCompleto = 'Cortina, Carlos Martin' and Cargo = 'Jefe') 
BEGIN INSERT INTO LiberadorSap (Mail, NombreCompleto, Obligatorio, Habilitado, Cargo, LiberadorSapTipo_Id) VALUES ('carlos.cortina@molinosagro.com.ar', 'Cortina, Carlos Martin', 0, 1, 'Jefe', 1) END
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.LiberadorSap WHERE Mail = 'dante.palmese@molinosagro.com.ar' and NombreCompleto = 'Palmese, Dante Leonel' and Cargo = 'Jefe') 
BEGIN INSERT INTO LiberadorSap (Mail, NombreCompleto, Obligatorio, Habilitado, Cargo, LiberadorSapTipo_Id) VALUES ('dante.palmese@molinosagro.com.ar', 'Palmese, Dante Leonel', 0, 1, 'Jefe', 1) END
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.LiberadorSap WHERE Mail = 'angel.lilli@molinosagro.com.ar' and NombreCompleto = 'Lilli, Angel' and Cargo = 'Jefe') 
BEGIN INSERT INTO LiberadorSap (Mail, NombreCompleto, Obligatorio, Habilitado, Cargo, LiberadorSapTipo_Id) VALUES ('angel.lilli@molinosagro.com.ar', 'Lilli, Angel', 0, 1, 'Jefe', 1) END
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.LiberadorSap WHERE Mail = 'diego.martin@molinosagro.com.ar' and NombreCompleto = 'Martin, Diego' and Cargo = 'Jefe') 
BEGIN INSERT INTO LiberadorSap (Mail, NombreCompleto, Obligatorio, Habilitado, Cargo, LiberadorSapTipo_Id) VALUES ('diego.martin@molinosagro.com.ar', 'Martin, Diego', 0, 1, 'Jefe', 1) END
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.LiberadorSap WHERE Mail = 'juan.carlos.fernandez@molinosagro.com.ar' and NombreCompleto = 'Fernandez, Juan Carlos' and Cargo = 'Jefe') 
BEGIN INSERT INTO LiberadorSap (Mail, NombreCompleto, Obligatorio, Habilitado, Cargo, LiberadorSapTipo_Id) VALUES ('juan.carlos.fernandez@molinosagro.com.ar', 'Fernandez, Juan Carlos', 0, 1, 'Jefe', 1) END
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.LiberadorSap WHERE Mail = 'marcelo.torrisi@molinosagro.com.ar' and NombreCompleto = 'Torrisi, Marcelo' and Cargo = 'Jefe') 
BEGIN INSERT INTO LiberadorSap (Mail, NombreCompleto, Obligatorio, Habilitado, Cargo, LiberadorSapTipo_Id) VALUES ('marcelo.torrisi@molinosagro.com.ar', 'Torrisi, Marcelo', 0, 1, 'Jefe', 1) END
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.LiberadorSap WHERE Mail = 'cintia.maltoni@molinosagro.com.ar' and NombreCompleto = 'Maltoni, Cintia' and Cargo = 'Jefe') 
BEGIN INSERT INTO LiberadorSap (Mail, NombreCompleto, Obligatorio, Habilitado, Cargo, LiberadorSapTipo_Id) VALUES ('cintia.maltoni@molinosagro.com.ar', 'Maltoni, Cintia', 0, 1, 'Jefe', 1) END
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.LiberadorSap WHERE Mail = 'luisina.vitorhoren@molinosagro.com.ar' and NombreCompleto = 'Vitor Horen, Luisina' and Cargo = 'Jefe') 
BEGIN INSERT INTO LiberadorSap (Mail, NombreCompleto, Obligatorio, Habilitado, Cargo, LiberadorSapTipo_Id) VALUES ('luisina.vitorhoren@molinosagro.com.ar', 'Vitor Horen, Luisina', 0, 1, 'Jefe', 1) END
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.LiberadorSap WHERE Mail = 'daniel.engel@molinosagro.com.ar' and NombreCompleto = 'Engel, Daniel' and Cargo = 'Jefe') 
BEGIN INSERT INTO LiberadorSap (Mail, NombreCompleto, Obligatorio, Habilitado, Cargo, LiberadorSapTipo_Id) VALUES ('daniel.engel@molinosagro.com.ar', 'Engel, Daniel', 0, 1, 'Jefe', 1) END
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.LiberadorSap WHERE Mail = 'lucas.cabral@molinosagro.com.ar' and NombreCompleto = 'Cabral, Lucas Adrian' and Cargo = 'Jefe') 
BEGIN INSERT INTO LiberadorSap (Mail, NombreCompleto, Obligatorio, Habilitado, Cargo, LiberadorSapTipo_Id) VALUES ('lucas.cabral@molinosagro.com.ar', 'Cabral, Lucas Adrian', 0, 1, 'Jefe', 1) END
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.LiberadorSap WHERE Mail = 'ivana.odone@molinosagro.com.ar' and NombreCompleto = 'Odone, Ivana Natali' and Cargo = 'Jefe') 
BEGIN INSERT INTO LiberadorSap (Mail, NombreCompleto, Obligatorio, Habilitado, Cargo, LiberadorSapTipo_Id) VALUES ('ivana.odone@molinosagro.com.ar', 'Odone, Ivana Natali', 0, 1, 'Jefe', 1) END
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.LiberadorSap WHERE Mail = 'edgardo.rios@molinosagro.com.ar' and NombreCompleto = 'Ríos, Edgardo' and Cargo = 'Jefe') 
BEGIN INSERT INTO LiberadorSap (Mail, NombreCompleto, Obligatorio, Habilitado, Cargo, LiberadorSapTipo_Id) VALUES ('edgardo.rios@molinosagro.com.ar', 'Ríos, Edgardo', 0, 1, 'Jefe', 1) END
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.LiberadorSap WHERE Mail = 'fernando.poncedeleon@molinosagro.com.ar' and NombreCompleto = 'Ponce de León, Fernando' and Cargo = 'Jefe') 
BEGIN INSERT INTO LiberadorSap (Mail, NombreCompleto, Obligatorio, Habilitado, Cargo, LiberadorSapTipo_Id) VALUES ('fernando.poncedeleon@molinosagro.com.ar', 'Ponce de León, Fernando', 0, 1, 'Jefe', 1) END
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.LiberadorSap WHERE Mail = 'patricio.manna@molinosagro.com.ar' and NombreCompleto = 'Manna, Patricio' and Cargo = 'Jefe') 
BEGIN INSERT INTO LiberadorSap (Mail, NombreCompleto, Obligatorio, Habilitado, Cargo, LiberadorSapTipo_Id) VALUES ('patricio.manna@molinosagro.com.ar', 'Manna, Patricio', 0, 1, 'Jefe', 1) END
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.LiberadorSap WHERE Mail = 'federico.pappo@molinosagro.com.ar' and NombreCompleto = 'Pappo, Federico' and Cargo = 'Jefe') 
BEGIN INSERT INTO LiberadorSap (Mail, NombreCompleto, Obligatorio, Habilitado, Cargo, LiberadorSapTipo_Id) VALUES ('federico.pappo@molinosagro.com.ar', 'Pappo, Federico', 0, 1, 'Jefe', 1) END
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.LiberadorSap WHERE Mail = 'matias.dangelo@molinosagro.com.ar' and NombreCompleto = 'DAngelo, Matias Ezequiel' and Cargo = 'Jefe') 
BEGIN INSERT INTO LiberadorSap (Mail, NombreCompleto, Obligatorio, Habilitado, Cargo, LiberadorSapTipo_Id) VALUES ('matias.dangelo@molinosagro.com.ar', 'DAngelo, Matias Ezequiel', 0, 1, 'Jefe', 1) END
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.LiberadorSap WHERE Mail = 'omar.mazany@molinosagro.com.ar' and NombreCompleto = 'Mazany, Omar' and Cargo = 'Director') 
BEGIN INSERT INTO LiberadorSap (Mail, NombreCompleto, Obligatorio, Habilitado, Cargo, LiberadorSapTipo_Id) VALUES ('omar.mazany@molinosagro.com.ar', 'Mazany, Omar', 0, 1, 'Director', 1) END
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.LiberadorSap WHERE Mail = 'ariel.lascano@molinosagro.com.ar' and NombreCompleto = 'Lascano, Ariel' and Cargo = 'Gerente') 
BEGIN INSERT INTO LiberadorSap (Mail, NombreCompleto, Obligatorio, Habilitado, Cargo, LiberadorSapTipo_Id) VALUES ('ariel.lascano@molinosagro.com.ar', 'Lascano, Ariel', 1, 1, 'Gerente', 1) END
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.LiberadorSap WHERE Mail = 'patricio.navarro@molinosagro.com.ar' and NombreCompleto = 'Navarro, Patricio' and Cargo = 'Gerente') 
BEGIN INSERT INTO LiberadorSap (Mail, NombreCompleto, Obligatorio, Habilitado, Cargo, LiberadorSapTipo_Id) VALUES ('patricio.navarro@molinosagro.com.ar', 'Navarro, Patricio', 0, 1, 'Gerente', 1) END
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.LiberadorSap WHERE Mail = 'ernesto.ventrici@molinosagro.com.ar' and NombreCompleto = 'Ventrici, Ernesto' and Cargo = 'Gerente') 
BEGIN INSERT INTO LiberadorSap (Mail, NombreCompleto, Obligatorio, Habilitado, Cargo, LiberadorSapTipo_Id) VALUES ('ernesto.ventrici@molinosagro.com.ar', 'Ventrici, Ernesto', 0, 1, 'Gerente', 1) END
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.LiberadorSap WHERE Mail = 'miguel.sarri@molinosagro.com.ar' and NombreCompleto = 'Sarri, Miguel Angel' and Cargo = 'Gerente') 
BEGIN INSERT INTO LiberadorSap (Mail, NombreCompleto, Obligatorio, Habilitado, Cargo, LiberadorSapTipo_Id) VALUES ('miguel.sarri@molinosagro.com.ar', 'Sarri, Miguel Angel', 0, 1, 'Gerente', 1) END
IF NOT EXISTS (SELECT TOP 1 1 FROM dbo.LiberadorSap WHERE Mail = 'juan.catala@molinosagro.com.ar' and NombreCompleto = 'Catala, Juan José' and Cargo = 'Gerente') 
BEGIN INSERT INTO LiberadorSap (Mail, NombreCompleto, Obligatorio, Habilitado, Cargo, LiberadorSapTipo_Id) VALUES ('juan.catala@molinosagro.com.ar', 'Catala, Juan José', 0, 1, 'Gerente', 1) END