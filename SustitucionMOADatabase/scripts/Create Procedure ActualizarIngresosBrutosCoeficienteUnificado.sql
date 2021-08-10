CREATE PROCEDURE ActualizarIngresosBrutosCoeficienteUnificado
    @IdIngresosBrutosCoeficienteUnificado int
AS   
    DECLARE @EstadoIngresosBrutosCoeficienteUnificado_Id_Autorizado INT = 2
    DECLARE @EstadoIngresosBrutosCoeficienteUnificado_Id_Completado INT = 3
 
    UPDATE IngresosBrutosCoeficienteUnificado 
    SET EstadoIngresosBrutosCoeficienteUnificado_Id = @EstadoIngresosBrutosCoeficienteUnificado_Id_Completado 
    WHERE Id = @IdIngresosBrutosCoeficienteUnificado 
    and EstadoIngresosBrutosCoeficienteUnificado_Id = @EstadoIngresosBrutosCoeficienteUnificado_Id_Autorizado
GO