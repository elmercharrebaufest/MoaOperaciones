-- Grupo de solicitantes externos para el jefe Diego Martin id=14647, mail='diego.martin@molinosagro.com.ar'
-- id=15545 , mail='veronica.rearte@molinosagro.com.ar'
-- id=15549 , mail='fernando.gomez@molinosagro.com.ar'
-- id=24156 , mail='Damian.Corgniali@molinosagro.com.ar'
-- id=25428 , mail='Ricardo.Lunarireynoso@molinosagro.com.ar'

-- Grupo de solicitantes externos para el jefe Marcelo Torrisi id=23482, mail='marcelo.torrisi@molinosagro.com.ar'
-- id=21254 , mail='julio.bragagnolo@molinosagro.com.ar'
-- id=21247 , mail='araceli.cerezo@molinosagro.com.ar'
-- id=23927 , mail='mateo.occhi@molinosagro.com.ar'
-- id=24287 , mail='Hernando.Serafin@molinosagro.com.ar'
-- id=25555 , mail='tomas.fortini@molinosagro.com.ar'

-- Crear el script en sql para buscar en la tabla Usuario el Id de la persona que será JefeMoa, guardar ese Id en una variable.
-- Luego buscar los Id de los usuarios haciendo un filtro por Mail para obtener el Id del usuario y así insertarlo en la tabla SoliciC:\Users\lnestares\Documents\Molinos Agro\MoaOperaciones\SustitucionMOADatabase\scripts\PostDeployment\RelacionSolicitanteExternoJefeMoa.sqltanteExternoJefeMoa
-- donde se inserta esta relación de Ids

-- Script para insertar relaciones entre solicitantes externos y sus jefes en la tabla RelacionSolicitanteExternoJefeMoa

-- Declaración de variables
DECLARE @JefeMoaId INT;
DECLARE @SolicitanteId INT;

-- Grupo 1: Jefe Diego Martin
-- Obtener el Id del Jefe
SELECT @JefeMoaId = Id FROM Usuario WHERE Mail = 'diego.martin@molinosagro.com.ar';

-- Insertar relaciones para los solicitantes de Diego Martin
-- Verónica Rearte
SELECT @SolicitanteId = Id FROM Usuario WHERE Mail = 'veronica.rearte@molinosagro.com.ar';
IF @SolicitanteId IS NOT NULL AND @JefeMoaId IS NOT NULL
    INSERT INTO RelacionSolicitanteExternoJefeMoa (Usuario_Id, JefeMoa_Id)
    VALUES (@SolicitanteId, @JefeMoaId);

-- Fernando Gómez
SELECT @SolicitanteId = Id FROM Usuario WHERE Mail = 'fernando.gomez@molinosagro.com.ar';
IF @SolicitanteId IS NOT NULL AND @JefeMoaId IS NOT NULL
    INSERT INTO RelacionSolicitanteExternoJefeMoa (Usuario_Id, JefeMoa_Id)
    VALUES (@SolicitanteId, @JefeMoaId);

-- Damian Corgniali
SELECT @SolicitanteId = Id FROM Usuario WHERE Mail = 'Damian.Corgniali@molinosagro.com.ar';
IF @SolicitanteId IS NOT NULL AND @JefeMoaId IS NOT NULL
    INSERT INTO RelacionSolicitanteExternoJefeMoa (Usuario_Id, JefeMoa_Id)
    VALUES (@SolicitanteId, @JefeMoaId);

-- Ricardo Lunarireynoso
SELECT @SolicitanteId = Id FROM Usuario WHERE Mail = 'Ricardo.Lunarireynoso@molinosagro.com.ar';
IF @SolicitanteId IS NOT NULL AND @JefeMoaId IS NOT NULL
    INSERT INTO RelacionSolicitanteExternoJefeMoa (Usuario_Id, JefeMoa_Id)
    VALUES (@SolicitanteId, @JefeMoaId);

-- Grupo 2: Jefe Marcelo Torrisi
-- Obtener el Id del Jefe
SELECT @JefeMoaId = Id FROM Usuario WHERE Mail = 'marcelo.torrisi@molinosagro.com.ar';

-- Insertar relaciones para los solicitantes de Marcelo Torrisi
-- Julio Bragagnolo
SELECT @SolicitanteId = Id FROM Usuario WHERE Mail = 'julio.bragagnolo@molinosagro.com.ar';
IF @SolicitanteId IS NOT NULL AND @JefeMoaId IS NOT NULL
    INSERT INTO RelacionSolicitanteExternoJefeMoa (Usuario_Id, JefeMoa_Id)
    VALUES (@SolicitanteId, @JefeMoaId);

-- Araceli Cerezo
SELECT @SolicitanteId = Id FROM Usuario WHERE Mail = 'araceli.cerezo@molinosagro.com.ar';
IF @SolicitanteId IS NOT NULL AND @JefeMoaId IS NOT NULL
    INSERT INTO RelacionSolicitanteExternoJefeMoa (Usuario_Id, JefeMoa_Id)
    VALUES (@SolicitanteId, @JefeMoaId);

-- Mateo Occhi
SELECT @SolicitanteId = Id FROM Usuario WHERE Mail = 'mateo.occhi@molinosagro.com.ar';
IF @SolicitanteId IS NOT NULL AND @JefeMoaId IS NOT NULL
    INSERT INTO RelacionSolicitanteExternoJefeMoa (Usuario_Id, JefeMoa_Id)
    VALUES (@SolicitanteId, @JefeMoaId);

-- Hernando Serafin
SELECT @SolicitanteId = Id FROM Usuario WHERE Mail = 'Hernando.Serafin@molinosagro.com.ar';
IF @SolicitanteId IS NOT NULL AND @JefeMoaId IS NOT NULL
    INSERT INTO RelacionSolicitanteExternoJefeMoa (Usuario_Id, JefeMoa_Id)
    VALUES (@SolicitanteId, @JefeMoaId);

-- Tomas Fortini
SELECT @SolicitanteId = Id FROM Usuario WHERE Mail = 'tomas.fortini@molinosagro.com.ar';
IF @SolicitanteId IS NOT NULL AND @JefeMoaId IS NOT NULL
    INSERT INTO RelacionSolicitanteExternoJefeMoa (Usuario_Id, JefeMoa_Id)
    VALUES (@SolicitanteId, @JefeMoaId);

