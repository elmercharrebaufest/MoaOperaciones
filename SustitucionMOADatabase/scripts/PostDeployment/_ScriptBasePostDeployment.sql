/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

:r .\EstadoIngresosBrutosCoeficienteUnificado.sql
:r .\OrigenMovimientoIngresosBrutosCoeficienteUnificado.sql
:r .\TipoMovimientoIngresosBrutosCoeficienteUnificado.sql
:r .\TipoImputacionSAP.sql
:r .\TipoSolpPosicionSAP.sql
:r .\PermisosNotificaciones.sql
:r .\PermisoPorRolBuscadorInteligente.sql