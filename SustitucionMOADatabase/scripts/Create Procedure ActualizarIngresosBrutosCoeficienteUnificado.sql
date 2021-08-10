CREATE PROCEDURE ActualizarIngresosBrutosCoeficienteUnificado
    @IdIngresosBrutosCoeficienteUnificado int
AS   
    DECLARE @EstadoIngresosBrutosCoeficienteUnificado_Id_Autorizado INT = 2
    DECLARE @EstadoIngresosBrutosCoeficienteUnificado_Id_Completado INT = 3
    DECLARE @EstadoConsultaCerrado INT = 5
    
    UPDATE IngresosBrutosCoeficienteUnificado 
    SET EstadoIngresosBrutosCoeficienteUnificado_Id = @EstadoIngresosBrutosCoeficienteUnificado_Id_Completado 
    WHERE Id = @IdIngresosBrutosCoeficienteUnificado 
    and EstadoIngresosBrutosCoeficienteUnificado_Id = @EstadoIngresosBrutosCoeficienteUnificado_Id_Autorizado

    UPDATE c
    SET EstadoConsulta_Id = @EstadoConsultaCerrado
    FROM Consulta c
    JOIN IngresosBrutosCoeficienteUnificado i ON c.Id = i.Consulta_Id
    WHERE i.Id = @IdIngresosBrutosCoeficienteUnificado
GO